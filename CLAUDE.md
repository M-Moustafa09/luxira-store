# CLAUDE.md — Luxira / Lotus Blue Backend

هذا الملف بيوضح للـ AI agent (Claude Code) السياق والمعايير الإلزامية لمشروع Luxira (Lotus Blue Storefront). المشروع لسه في مرحلة البداية (من الصفر)، فالهدف هو بناء أساس نظيف وقابل للتوسع من أول commit.

## مبدأ أساسي يحكم كل قرار في المشروع

- **ممنوع أي تعقيد زيادة عن اللزوم (No Over-Engineering)**: أي pattern أو library أو layer إضافية لازم يكون ليها سبب واضح ومباشر، مش "علشان ممكن تلزم مستقبلاً". لو فيه حل أبسط بيؤدي نفس الغرض بنفس الكفاءة، الأبسط هو المطلوب.
- **التنظيم والوضوح قبل أي حاجة تانية**: كل جزء من المشروع (فولدرات، تسمية، تقسيم الطبقات) لازم يكون واضح ومنظم ومتبع بالظبط الخطة الموضوعة في الملف ده — مفيش ارتجال أو تنفيذ خارج عن المتفق عليه من غير سؤال الأول.
- **البرفورمانس هي الأولوية القصوى في المشروع ده** — مش تفصيل يتراجع له بعدين، وأي قرار (سواء معماري أو حتى بسيط في كتابة query) لازم يتقيّم من زاوية تأثيره على الأداء قبل أي حاجة تانية.

## نظرة عامة على المشروع

- **الاسم**: Luxira — Lotus Blue Storefront Backend
- **النطاق**: منصة تجارة إلكترونية لمستحضرات التجميل، السوق المستهدف السعودية
- **الـ Frontend**: فريق منفصل بيبني admin dashboard + storefront (Angular على الأرجح) — لازم الـ Backend يوفر **Admin API كامل من البداية**، مش بس Storefront API
- **الـ Stack**: ASP.NET Core (أحدث LTS)، Entity Framework Core، SQL Server، Angular (استهلاك الـ API)

## المعمارية (إلزامية)

استخدم **Clean Architecture** بأربع طبقات منفصلة في مشاريع مستقلة:

```
Luxira.Domain          -> Entities, Value Objects, Domain Events, Interfaces (لا dependencies خارجية)
Luxira.Application     -> Service Interfaces, DTOs, Validators, Business Logic Contracts
Luxira.Infrastructure   -> EF Core, Repositories, Service Implementations, External Services (Payment, Email, Storage)
Luxira.API              -> Controllers, Middleware, DI Composition Root
```

قواعد صارمة (ملاحظة: **مفيش CQRS ولا MediatR في المشروع ده** — نستخدم Service Layer العادية):
- **Service Layer Pattern**: كل feature ليها Service Class خاصة بيها (مثلاً `IProductService` / `ProductService`) بتحتوي على الـ business logic، ومحقونة عبر DI في الـ Controllers مباشرة. ممنوع منطق business جوه الـ Controllers.
- كل Service بيتقسم لو كبر — مثلاً `ProductQueryService` للقراءة و `ProductCommandService` للكتابة لو الـ feature معقدة، لكن من غير Mediator/Pipeline، مجرد استدعاء مباشر (direct method calls).
- **Repository + Unit of Work pattern** فوق EF Core — الـ Application layer ميعرفش EF Core خالص (Dependency Inversion).
- **FluentValidation** لكل Request/Input Model، الـ validation يتنفذ صراحة داخل الـ Service قبل تنفيذ العملية (مش عبر Pipeline).
- **AutoMapper أو Mapster** للتحويل بين Entities والـ DTOs.
- كل Endpoint في الـ Controller بيرجع نتيجة موحدة (Result Pattern أو ProblemDetails للأخطاء) — مش exceptions عارية للـ client.

## الأمان (Security — غير قابل للتفاوض)

- **Authentication**: JWT بسيط (بدون ASP.NET Core Identity) — بس لازم يشمل:
  - Access Token قصير العمر (15-30 دقيقة) + Refresh Token طويل العمر مخزن بأمان (hashed في الداتابيز، مش plain text)
  - Endpoint منفصل لـ `/auth/refresh` و `/auth/logout` (بيلغي الـ refresh token)
  - كلمات السر مشفرة بـ BCrypt أو Argon2 — ممنوع أي hashing ضعيف
- **Authorization**: Role-based (على الأقل Admin / Customer) عبر `[Authorize(Roles = "...")]`، وكل Admin endpoint لازم يكون محمي بشكل صريح
- **Secrets**: أبداً في appsettings.json أو الكود. استخدم **User Secrets** في التطوير، و **Environment Variables / Azure Key Vault** في الإنتاج
- **Input Validation**: كل input من الـ client يتم التحقق منه بـ FluentValidation قبل ما يوصل للـ business logic
- **Rate Limiting**: مفعّل على الأقل على `/auth/*` endpoints لمنع brute-force
- **HTTPS**: إجباري، مع `UseHsts()` في الإنتاج
- **CORS**: محدد بدومينات معروفة فقط (frontend الـ Admin + Storefront)، ممنوع `AllowAnyOrigin` في الإنتاج
- **SQL Injection**: EF Core بيحمي تلقائياً طالما مفيش raw SQL — لو احتجت raw SQL استخدم parameterized queries فقط

## البرفورمانس

- **Async/Await** في كل عملية I/O (database, external APIs) — ممنوع `.Result` أو `.Wait()`
- **Pagination** إجباري على أي endpoint بيرجع list (Skip/Take أو Cursor-based للجداول الكبيرة)
- **AsNoTracking()** لكل query للقراءة فقط (معظم الـ Queries في الـ Storefront)
- **Indexing**: على الأقل indexes على Foreign Keys وأي عمود بيتفلتر أو بيترتب عليه كتير (مثلاً ProductName, CategoryId, Email)
- **Caching**: Response caching أو in-memory caching للبيانات اللي بتتغير قليل (Categories, Product listings)
- **Avoid N+1 Queries**: استخدم `.Include()` بذكاء أو Projection مباشر للـ DTO بدل تحميل الـ Entity كامل

## معايير عامة للكود

- **Naming**: قياسي .NET (PascalCase للـ classes/methods, camelCase للـ parameters/local variables), جداول الداتابيز بصيغة الجمع (Products, Orders)
- **Global Exception Handling** Middleware بيرجع ProblemDetails موحد
- **Logging**: Serilog، مع structured logging، ممنوع تسجيل بيانات حساسة (passwords, tokens)
- **API Documentation**: Swagger/OpenAPI مفعّل مع أمثلة على كل endpoint
- **Testing**: Unit tests للـ Application layer (Handlers) باستخدام xUnit + Moq/NSubstitute + FluentAssertions

## نقاط لسه معلقة (لا تفترض حلول لها من نفسك — اسأل قبل التنفيذ)

- **Payment Gateway**: لسه معلق، منتظر قرار المدير — ابني الـ Infrastructure على interface `IPaymentGateway` عشان يسهل الاستبدال لاحقاً
- **Admin Dashboard**: فريق منفصل بيبنيه، فالـ Admin API لازم يكون documented كويس وresponses مستقرة من البداية عشان مايكسرش شغلهم لاحقاً
- **CRM**: مش مطلوب دلوقتي، ممكن يضاف مستقبلاً — متبنيش أي جداول أو منطق ليه دلوقتي، بس لو صممت الـ Domain بشكل نظيف (مثلاً Customer entity منفصلة ومرنة) هيسهل إضافته لاحقاً من غير refactor كبير

## اللغة (Language Scope)

- **حالياً**: المشروع باللغة العربية فقط. مفيش أي دعم multi-language أو localization tables دلوقتي — امنع أي تعقيد زيادة في تصميم الجداول أو الـ DTOs بسبب اللغات.
- كل الـ Content (أسماء المنتجات، الأوصاف، رسائل الأخطاء المرسلة للـ client) بالعربي مباشرة كـ plain text في الأعمدة، من غير جداول ترجمة منفصلة (لا `ProductTranslations` ولا أي حاجة شبهها).
- لو احتجنا Multi-language مستقبلاً، هيتاخد قرار منفصل وقتها ونعمل migration مخصص — من غير over-engineering دلوقتي.

## طريقة العمل مع الـ Agent (إلزامي — اتبع الترتيب ده بالظبط)

### 1. الفهم الكامل قبل أي كود
قبل ما تكتب سطر كود واحد:
- ادرس الـ **Frontend الموجود بالكامل** (Storefront + Admin Dashboard) — افهم كل شاشة، كل API call متوقع، شكل الـ Mock Data المستخدم دلوقتي في الفرونت، وكل الـ endpoints اللي الفرونت محتاجها
- اطلع Inventory كامل: قائمة بكل الـ Entities المطلوبة، كل الـ Endpoints المطلوبة (Storefront + Admin)، وأي Mock Data موجودة في الفرونت لازم تتستبدل بيانات حقيقية من الداتابيز
- اعرض عليّ ملخص الفهم ده الأول قبل ما تبدأ التنفيذ — متبدأش كود من غير ما أوافق على الفهم

### 2. التقسيم لأجزاء صغيرة
- بعد الفهم الكامل، قسّم المشروع لـ modules/features صغيرة (مثلاً: Auth, Products, Categories, Orders, Cart...)
- كل module له خطة تنفيذ منفصلة: Entity -> Repository -> Service -> Controller -> ربط بالفرونت
- اعرض الخطة المقسّمة قبل البدء في التنفيذ الفعلي

### 3. التنفيذ خطوة بخطوة (Task by Task)
- **ممنوع تنفيذ أكتر من feature واحدة في نفس الوقت.** خلص الـ module كامل (من الـ Entity لحد ما يشتغل مع الفرونت فعلياً) قبل ما تنتقل للي بعده
- بعد كل خطوة، وقف واعرض اللي اتعمل، وانتظر مراجعة/موافقة قبل الخطوة اللي بعدها
- لما توصل لأي شاشة في الفرونت بتستخدم Mock Data، الأولوية إنك تستبدلها ببيانات حقيقية من الـ API فوراً — مفيش Mock Data تفضل موجودة بعد ما الـ endpoint بتاعها يخلص

### 4. البرفورمانس هي الأولوية القصوى
في كل خطوة وكل feature، البرفورمانس مش تفصيل يتراجع بعدين — لازم تتفحص من البداية:
- كل query بيترجع لل frontend لازم يتقاس زمن تنفيذه، ومفيش query بيرجع بيانات أكتر من اللازم (over-fetching)
- أي endpoint هيتنادى كتير من الفرونت (زي Product listing) — لازم يتاخد فيه بالذات وقت إضافي في التحسين (indexing, projection, caching) قبل الانتقال للي بعده
- قبل ما تعتبر أي module "خلص"، اعمل مراجعة برفورمانس سريعة عليه (N+1 queries؟ missing indexes؟ unnecessary tracking؟) وأنا هراجعها معاك

### 5. القرارات المعمارية الكبيرة
أي قرار كبير (اختيار caching provider، تصميم جدول مهم، إلخ) — اسأل الأول، متفترضش من نفسك.

## حالة المشروع الحالية (Living Status)

**آخر مراجعة حالة كاملة (status review): 2026-08-12**

هذا القسم بيتحدّث باستمرار مع تقدم المشروع، عشان أي session جديد (حتى لو fresh تماماً) يقدر يكمل من غير ما يعيد اكتشاف القرارات دي من الكود. القسم ده snapshot لـ"إحنا فين دلوقتي" — مش سجل تاريخي لكل تقرير اتعمل.

### الموديولات المكتملة (Storefront) ✅
1. **Categories**
2. **Products** — Entity موحّد (`Product` + `ProductVariant`)، وده حل مشكلة id-mismatch اللي كانت موجودة في الـ mock الأصلي
3. **Cart** — مربوط بـ guest identity (تفاصيل تحت)، شامل Bundle→Cart (تفاصيل تحت)
4. **Wishlist**
5. **Bundles & Offers** — شامل حساب خصم Coupon حقيقي (مش mock)
6. **Checkout & Orders**
7. **Reviews** — كـ entity اسمه `Testimonial` مش `Review` (السبب تحت)
8. **Account** — Profile، Addresses (CRUD)، Order History. التفاصيل تحت.
9. **Cart notifications** — Toast تأكيد عند الإضافة/الحذف (منتج أو Bundle)، أي مكان في الموقع.
10. **تأكيد حذف من السلة** — Bottom sheet ("هل أنتِ متأكدة...؟") قبل حذف أي منتج أو Bundle من السلة، نفس شكل bottom sheets التانية في الموقع (`ConfirmSheet.jsx`).
11. **Auth (Backend + Frontend)** — كامل، تفاصيله تحت.

### فلاتر صفحة Products ✅
- الفلاتر الأربعة (العلامة/Brand، السعر/Price، التقييم/Rating، نوع البشرة/SkinType) اتبنت بالكامل Backend + Frontend ومتأكد منها live في المتصفح.
- **Brand**: entity حقيقي جديد (`Luxira.Domain/Entities/Brand.cs`) زي `Category` بالظبط، مش enum ولا string خام. اتعمله migration (`AddBrandsAndProductFilters`) شاملة seeding (`DbSeeder.SeedBrandsAsync`) وbackfill غير مشروط (`BackfillProductBrandsAsync`) بيربط المنتجات القديمة بالـ Brand الصح كل startup لحد ما تتظبط، من غير ما يكسر منتجات اتعمل لها ربط قبل كده.
- **Price**: نطاق حر (`MinPrice`/`MaxPrice`) مش قيم محددة مسبقاً.
- **Rating**: threshold أدنى بس (`MinRating`) مش نطاق كامل — القرار كان إن ده أبسط وكافي للاستخدام الفعلي.
- **SkinType**: `enum` nullable على `Product` (`Luxira.Domain/Entities/SkinType.cs`) مش entity/جدول منفصل — لأنه taxonomy صغير وثابت ومش هيتضاف/يتعدل من الـ Admin زي الـ Category/Brand. **قرار متعمد**: مفيش أي بيانات SkinType متزروعة للمنتجات الحالية (كلها color cosmetics مالهاش علاقة حقيقية بنوع بشرة محدد) — الفلتر شغال فعلياً (بيرجع 0 منتج دلوقتي لأي نوع بشرة، ده صح) لحد ما يبقى فيه منتجات skincare حقيقية ليها بيانات صح تتزرع.
- الـ `IProductRepository.SearchAsync` اتبنى على `ProductSearchCriteria` (Domain type) بدل ما الـ method signature يكبر أكتر من اللازم (كان وصل لـ 11 parameter).
- Frontend: `OptionsBottomSheet.jsx` (component عام حل محل `CategoryBottomSheet.jsx` القديم) بيغطي Category/Brand/Rating/SkinType كلهم بنفس الشكل، و`PriceBottomSheet.jsx` منفصل للـ range input. مفيش pattern UI جديد اتضاف.

### Module 8: Account ✅
- **Profile**: `Customer.Name`/`Phone`/`Email` بقت قابلة للعرض والتعديل عن طريق `GET/PUT /api/customers/me` (`CustomerService`). أول قراءة للبروفايل بتعمل `GetOrCreateGuestAsync` زي أي مكان تاني بيستخدم `ICurrentUserService.CustomerId` — نفس الـ pattern المتبع في Cart/Wishlist.
- **Addresses**: entity جديد `CustomerAddress` (Domain) — نفس شكل الحقول اللي بيجمعها الـ Checkout بالظبط (`FullName`, `Phone`, `City`, `Region`, `AddressDetails`) عشان لو حبينا نربطها بالـ Checkout مستقبلاً (اختيار عنوان محفوظ بدل كتابته من الأول) مفيش mismatch في الشكل. مفهوم "عنوان افتراضي واحد بس" (`IsDefault`) متطبق عن طريق `IAddressRepository.ClearDefaultAsync` بيتنادى قبل أي set جديد. Endpoints: `GET/POST/PUT/DELETE /api/addresses`.
- **Order History**: `IOrderRepository.GetByCustomerAsync` (paginated, `AsNoTracking`) + `GET /api/orders/mine`. الـ Frontend بيعيد استخدام `OrderStatusCard` (نفس الكومبوننت المستخدم في `/track-order`) لعرض كل طلب — مفيش UI pattern جديد اتضاف.
- **الـ 4 عناصر دول اتشالوا من قائمة الحساب بقرار من المستخدم** (مفيش backend concept ليهم خالص دلوقتي): درجاتي المحفوظة، المنتجات التي اشتريتها، كوبونات الخصم، الإشعارات. "منتجاتي المفضلة" اتحول لمجرد لينك على `/wishlist` الموجودة بالفعل بدل ما يتعمل له صفحة جديدة.
- "تسجيل الخروج" في صفحة الحساب دلوقتي حقيقي (Auth section تحت) — بيظهر بس للمستخدم المسجل دخول، ولضيف بيتحول لـ "تسجيل الدخول/إنشاء حساب" بدل منه.

### Bundle → Cart ✅
- زرار "أضيفي إلى السلة" على الـ Bundle cards (في صفحتي Offers و Bundles) بقى شغال. القرار كان **Option B**: `BundleCartItem` entity جديد، الـ Bundle بتفضل سطر واحد في السلة بسعرها الخاص (مش بتتفكك لمنتجات منفصلة) — التفاصيل والمقارنة مع البديل (توسيعها لـ N × CartItem) موثقة تحت في القرارات المهمة.
- Endpoints: `POST/DELETE /api/cart/bundle-items/{id}` — إضافة/حذف بس، مفيش تعديل كمية (نفس الزرار بيزود الكمية لو اتضغط تاني على نفس الـ Bundle).
- عند الـ Checkout، كل `BundleCartItem` بيتحول لـ `OrderItem` واحد باسم/صورة/سعر الـ Bundle نفسها (`OrderItem.BundleId` nullable للتتبع) — مفيش UI جديد اتضاف لعرض الطلبات.

### Auth ✅ (Backend + Frontend كاملين، Merged لـ main)
- **Backend**: `register`/`login`/`refresh`/`logout` كاملين. BCrypt للباسورد، JWT (access قصير + refresh token دوّار مخزن hashed). Register بيحوّل الـ guest `Customer` الحالي (اللي جاي من `X-Guest-Id`) لحساب مسجّل بدل ما يعمل row جديد (**Option B** — تفاصيل تحت)، فالـ cart/wishlist بتاعت الـ guest بتفضل معاه. JWT Bearer اتوصل في الـ pipeline، و`CurrentUserService` بيقرا الـ customer id من الـ JWT claim (`sub`) لو المستخدم مسجل دخول، ولو مفيش يرجع لـ `X-Guest-Id` زي الأول بالظبط — **مفيش `[Authorize]` على أي endpoint لسه**، بقرار متعمد، عشان الـ guest experience (تصفح/سلة/checkout) يفضل زي ما هو تماماً.
- **Frontend**: صفحة واحدة `/login` + `/register` (نفس الـ component `Auth.jsx`، toggle بين login/register — مش صفحتين منفصلتين، بقرار من المستخدم)، متبنية بنفس pattern الـ `CheckoutInput`/`SectionCard` الموجود بالفعل في الـ Checkout. `authStore.js` (zustand) بيدير الـ tokens، و`authToken.js` بيخزنهم في localStorage بنفس أسلوب `guestId.js`. `apiClient.js` بقى يبعت `Authorization: Bearer` لو فيه token، وبقى كمان يقرا رسالة الخطأ الحقيقية من الـ ProblemDetails response بدل رسالة generic (ده كان ناقص قبل كده، محدش كان بيشوف رسائل زي "كلمة المرور غلط").
- **Logout الحقيقي**: `Menu.jsx` و`AccountMenu.jsx` دلوقتي بيبدّلوا بين "تسجيل الدخول/إنشاء حساب" (guest) و"تسجيل الخروج" الحقيقي (متسجل دخول) في نفس المكان اللي كان فيه الـ Logout القديم الوهمي. الـ Logout الجديد بينادي `POST /api/auth/logout` فعلاً (كان قبل كده بس بيمسح الـ guest id من localStorage من غير ما يكلم الـ backend خالص) — و**بيمسح الـ JWT tokens بس، من غير ما يمسح الـ `X-Guest-Id`**، لأن بعد Option B الـ `CustomerId` = نفس الـ guest id، فمسحه كان هيسيب سلة الحساب يتيمة (نفس الـ bug اللي اتعمل الـ feature ده أصلاً عشان يصلحه).
- **ملاحظة تقنية**: `hydrate()` في `authStore.js` بتتنادى مرة واحدة عند فتح الموقع، بتحول الـ refresh token المخزن لـ access token جديد — عشان عميل راجع (مش أول مرة) يتحل عن طريق الـ JWT مش الـ guest-id fallback.
- **مؤجل بقرار**: دمج سلة الـ guest مع حساب مسجل عند الـ **login** (مش الـ register) — الحالة اللي فيها عميل عنده حساب بالفعل بس بيدخل من متصفح/جهاز فيه سلة guest تانية منفصلة. ده Option C من نقاش guest→auth transition، اتأجل عن قصد لأنه محتاج منطق دمج حقيقي (جمع الكميات، تعارض الكوبون لو الاتنين سلة ليهم كوبون مختلف) مش مجرد تحويل identity زي Option B.
- **لسه ناقص**: الـ `CustomerRole.Admin` موجود كـ enum وفيه admin customer متزروع للتست، بس مفيش أي `[Authorize(Roles = "Admin")]` بيستخدمه لسه (لسه مفيش Admin API يتحمي أصلاً).

### الموديولات المتبقية
- **❌ Admin API** — مش موجود خالص لسه (اتأكد بالبحث في الكود). معناه دلوقتي مفيش أي طريقة تدار بيها المتجر (إضافة منتج، متابعة طلب، تحديد stock) غير الدخول على الداتابيز مباشرة. القرار المتبع لسه: نضيفه module-by-module بعد كل storefront module.
- **❌ Payment Gateway** — لسه معلق تماماً، منتظر قرار المدير. مفيش أي `IPaymentGateway` interface أو أي كود دفع لسه (اتأكد بالبحث) — الخطة إن الـ checkout الحالي بيعمل Order من غير خطوة دفع فعلية.
- **⚠️ Stock/Inventory — أولوية عالية، لسه مش موجود**: مفيش أي مفهوم Stock/Inventory في المشروع كله (اتأكد بالبحث) — مش بس للـ Bundles، لأي منتج عادي كمان. أي عملية شراء ممكن تبيع أكتر من المتاح فعلياً من غير أي تحقق. الخطة المتفق عليها:
  - إضافة `Stock` (int) على `ProductVariant` (مش على `Product`) لأن الشراء بيحصل على مستوى الـ variant.
  - في `OrderService.CreateAsync`، جوه نفس الـ transaction: التحقق إن كل سطر (`CartItem` أو منتجات `BundleCartItem` — `BundleItem.Quantity × BundleCartItem.Quantity`) عنده Stock كافي، وخصمه، ورفض الطلب كله لو أي سطر مش متوفر.
  - محتاج كمان: تحديد الـ Stock من الـ Admin API (لسه مش موجودة)، وحالة "نفذت الكمية" في الـ Storefront.
  - cross-cutting، مش خاص بالـ Bundles بس — scoped work منفصل، اسأل قبل التنفيذ.
- **اللغة (Arabic/English) والعملة حسب الدولة**: اتأجلوا الاتنين بقرار من المستخدم — لسه في مرحلة investigation بس (مفيش تنفيذ). **تنبيه مهم**: النطاق اتوسّع من السعودية بس لحوالي 10 دول عربية بعملات مختلفة (EGP, SAR, AED, KWD, QAR...) — ده بيعمل تعارض مباشر مع قرار "اللغة (Language Scope)" فوق اللي بيمنع أي جداول ترجمة/تعقيد لغوي دلوقتي. لازم يتحسم الاتنين مع بعض قبل التنفيذ، مش يتاخدوا كقرارين منفصلين.

### Production-Readiness — الحالة الفعلية بعد المراجعة
| البند | الحالة |
|---|---|
| Rate Limiting على `/auth/*` | ❌ مش موجود — `/auth/login` و `/auth/register` مفتوحين لـ brute-force دلوقتي |
| Structured Logging (Serilog) | 🟡 جزئي — Serilog شغال، بس Console sink بس، مفيش persistence لأي مكان تاني |
| Unit Tests | ❌ مفيش test project خالص في الـ solution |
| HTTPS / HSTS | 🟡 جزئي — `UseHttpsRedirection()` موجود، `UseHsts()` مش موجود |
| CORS | 🟡 dev-only — `http://localhost:5173` بس، مفيش domain إنتاج لسه (متوقع، مفيش إنتاج لسه) |
| Deployment / CI | ❌ مفيش خالص — لا `.github/workflows`، لا Dockerfile |

### الأولوية المقترحة لجاهزية المتجر لعملاء حقيقيين
بالترتيب من الأكتر حرجاً: **(1)** Admin API (مفيش طريقة تدار بيها المتجر) → **(2)** Stock/Inventory (ممكن يتباع أكتر من المتاح) → **(3)** Payment Gateway (الـ checkout مش بياخد فلوس فعلياً، منتظر قرار المدير) → **(4)** Rate Limiting على `/auth/*` + HSTS (فجوات أمان حقيقية دلوقتي بعد ما الـ Auth بقى customer-facing فعلاً) → **(5)** Tests + CI/Deployment. ~~ربط الـ Auth بالفرونت~~ **اتعمل ✅** (كان بند رقم 4 قبل كده). اللغة/العملة (Task 2/3) وOption C (guest cart merge on login) أقل حرجاً من دول التاني — مفيش منهم بيمنع عميل يتصفح/يسجل/يشتري.

### حالة الـ Branches
- `main` — up to date مع origin، وفيه كل حاجة اتعملت لحد دلوقتي (كل الـ storefront modules، Bundle→Cart، Cart notifications، تأكيد حذف من السلة، Auth backend + frontend).
- كل الـ feature branches السابقة (`feature/auth`, `feature/cart-notifications`, `feature/cart-remove-confirm`, `feature/auth-frontend`, `feature/bundle-to-cart`, `docs/status-update`) اتعملها merge بالكامل لـ `main` ومفيش commits قدامه. مفيش شغل معلق على branch منفصل دلوقتي إلا لو حاجة جديدة اتبدأت.

### قرارات مهمة لازم تتفتكر

**1. Auth: JWT + Guest-Id fallback (مش استبدال، الاتنين شغالين مع بعض):**
- `ICurrentUserService.CustomerId` بيقرا من الـ JWT `sub` claim لو موجود ومصدّق، ولو مفيش يرجع لـ `X-Guest-Id` header زي الأول بالظبط. مفيش `[Authorize]` على أي endpoint، فالـ resolution ده بيحصل دايماً بغض النظر عن وجود token.
- **ملاحظة تقنية مهمة**: لازم `MapInboundClaims = false` في إعدادات الـ JwtBearer، لأن الـ default handler بيستبدل الـ claim type بتاع "sub" لواحد تاني قديم (`ClaimTypes.NameIdentifier`) — من غيرها الـ lookup بيفشل بصمت.
- الفرونت بيعمل generate لـ guest GUID زي الأول (`src/lib/guestId.js`)، وكل الـ API calls بتبعته — ده لسه شغال لكل guest، وبقى فيه كمان `Authorization: Bearer` بيتبعت جنبه لو المستخدم مسجل دخول (تفاصيل الـ Frontend integration في قسم Auth فوق).

**2. Register بيحوّل الـ guest Customer بدل ما يعمل واحد جديد (Option B):**
- بدل ما `RegisterAsync` يعمل `Customer` جديد، بياخد الـ guest Customer الحالي (`GetOrCreateGuestAsync(_currentUser.CustomerId)`) ويحوّله (`IsGuest = false` + باقي البيانات) — نفس الـ `CustomerId`، فالـ cart/wishlist/addresses بتاعته بتفضل معاه تلقائي من غير أي دمج.
- لو نفس الـ guest id حاول يسجل تاني وهو بقى مسجل بالفعل، بيترفض برسالة واضحة ("هذا الحساب مسجل بالفعل").
- دمج سلة guest منفصلة وقت **login** (مش register) — ده Option C، اتأجل، تفاصيله فوق في "Auth".

**3. Admin API مؤجل بالكامل:**
- بناءً على طلب المستخدم، الأولوية كانت لاستبدال الـ mock data في الـ Storefront الأول قبل أي حاجة تانية.
- القرار: كل ما نخلص storefront module، نرجعله بعدين ونضيف الـ Admin endpoints بتاعته.

**4. Bundle → Cart: القرار اتاخد (Option B) واتنفذ:**
- زرار "أضيفي إلى السلة" على الـ Bundle cards كان مش شغال أصلاً (مطابق لسلوك الـ mock الأصلي، مفيش رجوع للخلف).
- اخترنا `BundleCartItem` entity جديد (الـ Bundle سطر واحد بسعرها الخاص) بدل توسيعها لـ N × CartItem، لسببين: الـ `BundleItem` مفيهوش `ProductVariantId` أصلاً، وسعر الـ Bundle مكتوب مباشرة مش محسوب من مجموع المنتجات (والسلة فيها كوبون واحد بس على مستواها). تفاصيل كاملة في commit `feature/bundle-to-cart`.
- Scope متعمد: إضافة/حذف بس، من غير تعديل كمية منفصل (الضغط تاني بيزود الكمية).

**5. ليه Testimonial مش Review:**
- الـ entity بتاع "آراء العملاء" في الصفحة الرئيسية اتسمى `Testimonial` عن قصد، مش `Review`، عشان لو حبينا نضيف "مراجعات لكل منتج" مستقبلاً، اسم `Review` هيبقى متاح ومناسب له.

**6. Cart Notifications: custom toast مش library جديدة:**
- Zustand store (`toastStore.js`) + framer-motion للـ animation — الاتنين موجودين في المشروع بالفعل، فمفيش داعي لـ dependency جديدة (زي react-hot-toast) عشان رسالتين بس.
- اتحط جوه `cartStore.js` نفسها (`addItem`/`removeItem`/`addBundleItem`/`removeBundleItem`) مش في كل مكان بيستخدمها، عشان كل زرار Add/Remove في الموقع (offers, bundles, product cards, cart) يغطى تلقائي من غير wiring في كل صفحة.

**7. تأكيد حذف من السلة: bottom sheet مش confirm() المتصفح:**
- `ConfirmSheet.jsx` بنفس شكل bottom sheets التانية الموجودة (`FilterBottomSheet`, `OptionsBottomSheet`) عشان يفضل نفس الـ UI pattern.
- Scope اتأكد بالبحث: `removeItem`/`removeBundleItem` بينادوا بس من `Cart.jsx` — مفيش مكان تاني في الموقع فيه زرار حذف من السلة، فالتعديل اتحصر في الصفحة دي بس.

**8. Auth Frontend: صفحة واحدة (login/register toggle) مش صفحتين، Logout الحقيقي بيمسح الـ JWT بس:**
- قرار من المستخدم: `Auth.jsx` واحد بيبدّل بين وضعين (`mode="login"` / `mode="register"`) بدل صفحتين منفصلتين — أقل تكرار كود، ونفس الفكرة اللي Checkout بيتبعها (صفحة مخصصة لفورم حقيقي، مش bottom sheet، لأنه فيه validation وأخطاء لازم تتعرض).
- `apiClient.js` اتعدّل عشان يقرا رسالة الخطأ الحقيقية من الـ backend (ProblemDetails) بدل رسالة generic — كان ده ناقص من قبل، وكل الصفحات التانية (زي Checkout) لسه بتستخدم رسائل generic في الـ catch بتاعتها، مش حاجة اتغيرت هنا.
- Logout الجديد بيمسح الـ JWT tokens بس (مش `X-Guest-Id`) — تفاصيل السبب فوق في قسم Auth.

### إعدادات البيئة المحلية (Local Dev)
- **Backend**: `http://localhost:5080` (متظبط في `launchSettings.json`، بروفايل "http" الافتراضي)
- **Frontend**: `http://localhost:5173` (Vite default)
- **Database**: SQL Server LocalDB، اسم الداتابيز `LuxiraDb`، الـ connection string متخزن في User Secrets مش appsettings.json
- **CORS**: مسموح بس لـ `http://localhost:5173`
- **Coupon تجريبي متزروع في الـ seed data**: `WELCOME10` (خصم 10%)
- لو حصل port collision (الباك إند مش شغال على 5080 أو الفرونت مش شغال على 5173)، الاحتمال الأكبر إن فيه process قديم من session سابقة لسه شغال على نفس البورت ولازم يتقفل الأول
