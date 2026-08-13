# CLAUDE.md — Luxira / Lotus Blue Backend

هذا الملف بيوضح للـ AI agent (Claude Code) السياق والمعايير الإلزامية لمشروع Luxira (Lotus Blue Storefront). المشروع لسه في مرحلة البداية (من الصفر)، فالهدف هو بناء أساس نظيف وقابل للتوسع من أول commit.

## مبدأ أساسي يحكم كل قرار في المشروع

- **ممنوع أي تعقيد زيادة عن اللزوم (No Over-Engineering)**: أي pattern أو library أو layer إضافية لازم يكون ليها سبب واضح ومباشر، مش "علشان ممكن تلزم مستقبلاً". لو فيه حل أبسط بيؤدي نفس الغرض بنفس الكفاءة، الأبسط هو المطلوب.
- **التنظيم والوضوح قبل أي حاجة تانية**: كل جزء من المشروع (فولدرات، تسمية، تقسيم الطبقات) لازم يكون واضح ومنظم ومتبع بالظبط الخطة الموضوعة في الملف ده — مفيش ارتجال أو تنفيذ خارج عن المتفق عليه من غير سؤال الأول.
- **البرفورمانس هي الأولوية القصوى في المشروع ده** — مش تفصيل يتراجع له بعدين، وأي قرار (سواء معماري أو حتى بسيط في كتابة query) لازم يتقيّم من زاوية تأثيره على الأداء قبل أي حاجة تانية.

## نظرة عامة على المشروع

- **الاسم**: Luxira — Lotus Blue Storefront Backend
- **النطاق**: منصة تجارة إلكترونية لمستحضرات التجميل، السوق المستهدف السعودية
- **الـ Frontend**: فريق منفصل بيبني admin dashboard + storefront (React + Vite — `lotus-blue`) — لازم الـ Backend يوفر **Admin API كامل من البداية**، مش بس Storefront API
- **الـ Stack**: ASP.NET Core (أحدث LTS)، Entity Framework Core، SQL Server، React + Vite (استهلاك الـ API)

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

**آخر مراجعة حالة كاملة (status review): 2026-08-13**

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
- **لسه ناقص**: الـ `CustomerRole.Admin` موجود كـ enum وفيه admin customer متزروع للتست، وبقى ليه استخدام حقيقي دلوقتي (Admin API تحت).

### Admin API 🟡 (جزئي — Modules 1-4 اتعملوا، الباقي لسه)
مبنية بنفس الـ Service/Repository الموجودين للـ Storefront (extend مش duplicate)، تحت `/api/admin/*`، كل controller عليه `[Authorize(Roles = "Admin")]` على مستوى الـ class. Admin تجريبي متزروع: `admin@luxira.sa` / `Admin@12345`.

1. **Module 1 — Admin auth wiring** ✅: `AdminController` (`GET /api/admin/ping`) بيتأكد إن الـ role-based auth شغال end-to-end. مفيش تعديل كان لازم على إصدار الـ JWT أو `Program.cs` — الـ role claim كان متضاف من زمان في `AuthService.IssueTokensAsync`.
2. **Module 2 — Admin Orders** ✅: `GET /api/admin/orders` (كل الطلبات لكل العملاء، paginated، فلترة بالـ status)، `GET /api/admin/orders/{id}`، `PUT /api/admin/orders/{id}/status` (بيحدّث `Order.Status` ويسجّل صف جديد في `OrderStatusHistory`، برفض إعادة نفس الحالة). **ده بيحل مشكلة "تتبع الطلب مجمّد" اللي كانت متوثقة قبل كده** — الحالة بقت فعلاً بتتقدم دلوقتي.
3. **Module 3 — Admin Products CRUD** ✅: `GET/POST/PUT/DELETE /api/admin/products` + `GET /api/admin/products/{id}` (بيعيد استخدام نفس بحث/فلترة الـ Storefront). الحذف بيرفض برسالة واضحة (مش 500) لو المنتج مستخدم في سلة/باقة حالية (FK Restrict). الـ Variants بتتستبدل بالكامل عند التعديل (replace-all، مش diff).
4. **Module 3b — Country Pricing (Product display)** ✅: تفاصيل كاملة تحت في قسم "Country Pricing".
5. **Module 3c — Country Pricing (Cart/Checkout/Order)** ✅: تفاصيل كاملة تحت في قسم "Country Pricing".
6. **Module 4 — Admin Categories/Brands CRUD** ✅: `GET/POST/PUT/DELETE /api/admin/categories` (الأقسام الفرعية بتتستبدل بالكامل عند التعديل، replace-all زي الـ Variants) و`GET/POST/PUT/DELETE /api/admin/brands`. الحذف بيرفض برسالة واضحة (مش 500) لو التصنيف/العلامة مستخدمة في منتجات حالية (FK Restrict) — نفس pattern حذف المنتجات.
7. **الباقي لسه** (بالترتيب المتفق عليه): Stock/Inventory field + admin editing → Image upload (local disk + `IStorageService`) → Coupons/Bundles/Campaigns/Testimonials CRUD.

### الموديولات المتبقية
- **🟡 Admin API** — جزئي، تفاصيل فوق. الباقي: Stock/Inventory admin editing، Image upload، Coupons/Bundles/Campaigns/Testimonials CRUD.
- **❌ Payment Gateway** — لسه معلق تماماً، منتظر قرار المدير. مفيش أي `IPaymentGateway` interface أو أي كود دفع لسه (اتأكد بالبحث) — الخطة إن الـ checkout الحالي بيعمل Order من غير خطوة دفع فعلية.
- **⚠️ Stock/Inventory — أولوية عالية، لسه مش موجود**: مفيش أي مفهوم Stock/Inventory في المشروع كله (اتأكد بالبحث) — مش بس للـ Bundles، لأي منتج عادي كمان. أي عملية شراء ممكن تبيع أكتر من المتاح فعلياً من غير أي تحقق. الخطة المتفق عليها:
  - إضافة `Stock` (int) على `ProductVariant` (مش على `Product`) لأن الشراء بيحصل على مستوى الـ variant.
  - في `OrderService.CreateAsync`، جوه نفس الـ transaction: التحقق إن كل سطر (`CartItem` أو منتجات `BundleCartItem` — `BundleItem.Quantity × BundleCartItem.Quantity`) عنده Stock كافي، وخصمه، ورفض الطلب كله لو أي سطر مش متوفر.
  - محتاج كمان: تحديد الـ Stock من الـ Admin Products CRUD (موجودة دلوقتي، محتاجة بس إضافة الحقل)، وحالة "نفذت الكمية" في الـ Storefront.
  - cross-cutting، مش خاص بالـ Bundles بس — scoped work منفصل، اسأل قبل التنفيذ.
- **⚠️ خيار "بطاقة" في الـ Checkout وهمي/مضلل للعميل**: الـ UI بيعرض خيار دفع بالبطاقة وكأنه شغال، بس فعلياً مفيش أي form لبيانات البطاقة ولا أي شحن فعلي — مجرد label متخزن على الـ Order من غير أي معالجة دفع حقيقية ورا الكواليس. لازم يتحل مع قرار Payment Gateway (أعلاه) — إما يتشال الخيار مؤقتاً لحد ما يبقى فيه payment حقيقي، أو يتوصل بـ gateway حقيقي.
- **✅ تتبع الطلب (Order Tracking) — اتحل**: كان مجمّد دايماً على "Confirmed" لأن مفيش حاجة في الكود كانت بتغيّر `OrderStatus`. اتحل عن طريق Admin Orders Module 2 (`PUT /api/admin/orders/{id}/status`) — تفاصيل فوق.
- **اللغة (Arabic/English)** — لسه مؤجلة بقرار من المستخدم، لسه في مرحلة investigation بس (مفيش تنفيذ)، ومنفصلة تماماً دلوقتي عن موضوع العملة (تحت).
- **العملة حسب الدولة — اتحل التصميم واتنفذ بالكامل (Module 3b + 3c، مش "لسه مؤجل" زي ما كان متوثق قبل كده)**: النطاق النهائي اتحدد بـ 16 دولة محددة بالاسم (مش ~10 تقريبية زي أول investigation) — تفاصيل كاملة في قسم "Country Pricing" تحت. **مهم**: موضوع العملة اتفصل تماماً عن موضوع اللغة (Arabic/English) — القرار كان إنهم مش لازم يتحسموا مع بعض زي ما كان متوقع قبل كده، العملة عندها حل مستقل دلوقتي.

### Country Pricing ✅ (Module 3b + 3c كاملين)
تسعير حسب الدولة لـ 16 دولة محددة بالاسم: الأردن، الإمارات، البحرين، الجزائر، السعودية، العراق، الكويت، المغرب، تركيا، تونس، عُمان، فلسطين، قطر، لبنان، ليبيا، مصر.

- **الداتا موديل**: `ProductCountryPrice` — one-to-many من `Product` (مش many-to-many حقيقي)، unique index على `(ProductId, Country)`. `Country` عبارة عن enum بـ16 قيمة ثابتة (نفس منطق قرار `SkinType` enum قبل كده) مش جدول lookup منفصل.
- **العملة مشتقة تلقائياً من الدولة** (`CountryCurrency.For(country)`)، **مش مدخلة يدوياً من الأدمن** — قرار اتاخد بعد نقاش، عشان يمنع خطأ زي إدخال سعر بالجنيه المصري وهو متعلّم بالريال السعودي غلط. فلسطين اتحطلها ILS كعملة افتراضية (افتراض محتاج تأكيد من صاحب المشروع، مش قرار نهائي 100%).
- **الـ USD/fallback price**: مفيش صف تحت مسمى "USD" في `ProductCountryPrice` — الحقول الموجودة أصلاً `Product.Price`/`Product.OldPrice` هي الـ USD/fallback، بتتستخدم لو الزائر برّه الـ16 دولة، أو لو الأدمن لسه ما دخلش سعر الدولة دي للمنتج ده.
- **النطاق**: Products بس دلوقتي. Bundles فاضلة بسعرها الثابت زي ما هي، Coupons مش متأثرة.
- **Resolution + Pinning**: `Customer.Country` (nullable enum) + `Customer.CountryResolvedAt` (nullable DateTime) — بيتحلوا مرة واحدة بس لكل عميل (زي فكرة الـ guest-id) عن طريق `ICountryResolver` (`Luxira.API/Services/CountryResolverService.cs`)، ومبيتغيروش تاني حتى لو الشبكة اتغيرت وسط الجلسة. `CountryResolvedAt` هو اللي بيفرّق بين "لسه ما اتحاولش" (null) و"اتحاول ولقى الزائر برّه الـ16 دولة" (Country = null بس CountryResolvedAt متسجل) — من غيره كنا هنعيد محاولة الـ IP lookup كل request للزوار البرّه القائمة.
- **الـ Geolocation — MaxMind GeoLite2، شغالة فعلياً دلوقتي (مش stub)**: `Luxira.Infrastructure/Services/GeoIpLookup.cs` بيستخدم package `MaxMind.GeoIP2` (NuGet) وملف حقيقي `GeoLite2-Country.mmdb` محطوط في `Luxira.API/App_Data/` (متسجل في `.gitignore` عن طريق `*.mmdb` — مايتعملوش commit أبداً، كل بيئة لازم تحط نسختها). المسار متظبط في `appsettings.Development.json` تحت `GeoIp:DatabasePath`. اتعمله اختبار حقيقي بـ IPs عامة (تونس/لبنان/الأردن اتعرفوا صح، أمريكا/بريطانيا رجعوا null زي المتوقع، الـ loopback رجع "not found" وده صح).
- **Dev override**: في بيئة الـ Development بس، `?country=Egypt` كـ query param أو header `X-Dev-Country` بيتخطى الـ IP lookup تماماً — لازم لأن أي طلب من localhost بيرجع IP خاص (private) مش هيتلاقاله بلد حقيقي في MaxMind.
- **الفشل/الغموض (VPN, proxy, IP خاص)**: منطق ثنائي بسيط — إما اتحلت لواحدة من الـ16 دولة، أو أي حاجة تانية (دولة برّه القائمة، فشل الـ lookup، IP خاص) بترجع USD. مفيش حالة تالتة "غامضة" منفصلة.
- **الفلترة بالسعر (`MinPrice`/`MaxPrice`) — فجوة معروفة ومتعمدة، لسه موجودة**: لسه شغالة على الـ base USD `Product.Price`، مش على سعر الدولة المحلي. مش جزء من Module 3c (Module 3c خاص بـ Cart/Checkout/Order بس، مش الفلترة/الترتيب في صفحة المنتجات).

**Module 3c — Cart/Checkout/Order ✅:**
- `CartItem` مبيخزنش سعره — بيتحسب live من `Product.Price` كل مرة السلة بتتقرا (زي ما كان قبل كده بالظبط)، فده خلى تطبيق تسعير الدولة عليه سهل: نفس أسلوب الـ overlay المستخدم في Module 3b (`CartService.ApplyCountryPricingAsync`) بيجيب سعر الدولة لكل منتج في السلة ويغيّر `UnitPrice`/`LineTotal` بعد الـ mapping مباشرة.
- **قاعدة "الكل أو لا حاجة" (all-or-nothing) — قرار مهم اتاخد بعد سؤال المستخدم**: السلة بتتسعّر بعملة الدولة المحلية **بس لو كل سطر فيها** (كل المنتجات) عنده سعر لنفس الدولة. لو أي منتج واحد لسه معندوش سعر لدولة الزائر، **السلة كلها** (مش السطر ده بس) بترجع للدولار. السبب: مفيش طريقة رياضياً صح تجمع سطرين بعملتين مختلفتين في `Subtotal`/`Total` واحد.
- **الباقات (Bundles) بتفرض USD على السلة كلها لو موجودة** — لأن الباقات برّه نطاق تسعير الدولة أصلاً (قرار Module 3b)، فأي سلة فيها باقة + منتجات، حتى لو المنتجات كلها متسعّرة صح لدولة الزائر، بترجع كلها للدولار. ده نتيجة مباشرة لقرار "Products بس" في Module 3b، مش حاجة جديدة.
- **`Order.Currency` — حقل جديد على `Order`**: لازم عشان `Order.Total` يبقى له وحدة واضحة (999 من غير عملة مبهم — دولار ولا جنيه ولا ريال؟). بيتسجل وقت الـ checkout من `cart.Currency` مباشرة (نفس القيمة اللي اتحسبت للسلة، من غير إعادة resolve). الطلبات القديمة (قبل الـ migration) اتعملها backfill لـ `"USD"` تلقائياً.
- **اتعمله اختبار كامل end-to-end**: سلة بمنتجين متسعّرين لمصر (EGP صح) → إضافة منتج مش متسعّر (رجعت السلة كلها USD) → شيل المنتج (رجعت EGP تاني) → إضافة باقة (رجعت USD تاني) → شيل الباقة وعمل checkout (الـ Order اتسجل بـ `Currency: "EGP"` والأرقام صح) → تأكيد إن الطلبات القديمة بترجع `"USD"`.

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
بالترتيب من الأكتر حرجاً: **(1)** Admin API — 🟡 اتبدأ فعلاً (Auth wiring + Orders + Products + Country Pricing + Categories/Brands)، باقي Stock/Images/Coupons-Bundles-Campaigns-Testimonials → **(2)** Stock/Inventory (ممكن يتباع أكتر من المتاح) → **(3)** Payment Gateway (الـ checkout مش بياخد فلوس فعلياً، منتظر قرار المدير) → **(4)** Rate Limiting على `/auth/*` + HSTS (فجوات أمان حقيقية دلوقتي بعد ما الـ Auth بقى customer-facing فعلاً) → **(5)** Tests + CI/Deployment. ~~ربط الـ Auth بالفرونت~~ **اتعمل ✅**. ~~تتبع الطلب مجمّد~~ **اتعمل ✅** (عن طريق Admin Orders). ~~Module 3c: Cart/Checkout/Order يستخدموا سعر الدولة~~ **اتعمل ✅**. اللغة (Task 2) وOption C (guest cart merge on login) أقل حرجاً من دول التاني — مفيش منهم بيمنع عميل يتصفح/يسجل/يشتري.

### حالة الـ Branches
- `main` — up to date مع origin لحد آخر push (commit `a384180`، Admin API Modules 1-3 + Country Pricing). كل الـ storefront modules، Bundle→Cart، Cart notifications، تأكيد حذف من السلة، Auth backend + frontend، Admin API + Country Pricing — كلهم متعملهم merge وموجودين ومدفوعين لـ origin.
- كل الـ feature branches السابقة (`feature/auth`, `feature/cart-notifications`, `feature/cart-remove-confirm`, `feature/auth-frontend`, `feature/bundle-to-cart`, `feature/admin-api-country-pricing`, `docs/status-update`, `docs/auth-status-update`) اتعملها merge بالكامل لـ `main` ومفيش commits قدامها. مفيش شغل معلق على branch منفصل دلوقتي.

### سياسة الـ Merge: محلي دلوقتي، PR لما نضيف CI أو contributor تاني
- كل feature لسه بتاخد branch منفصل، وبعد المراجعة في الـ chat بتتعمل merge **محلياً** (`git merge --no-ff`) لـ `main` وبعدين push — مش عن طريق GitHub PR.
- **السبب**: المشروع solo دلوقتي ومفيش CI configured (لا GitHub Actions ولا أي pipeline)، فالـ PR مش هيضيف حاجة فعلية — مفيش checks تلقائية تتنفذ عليه، والمراجعة أصلاً بتحصل في الـ chat قبل الـ merge (زي مراجعة PR بالظبط، بس من غير الخطوة الإضافية). الـ `--no-ff` بيحافظ على نفس شكل الـ history اللي كان هيطلع لو اتعمل merge عن طريق PR على GitHub.
- **نتحول لـ PR-based workflow (push الـ branch + merge عن طريق GitHub) لما يحصل أي واحد من الاتنين**: (1) يتضاف CI حقيقي (build/tests بتتشغل تلقائي) — ساعتها الـ PR بيبقى نقطة الـ gate الطبيعية، أو (2) يبقى فيه contributor تاني على المشروع محتاج يراجع قبل الـ merge. الاتنين دول من ضمن أولويات المشروع (Tests + CI/Deployment في الجدول فوق) — لما يوصلوا، السياسة دي لازم تتغير معاهم.

### قرارات مهمة لازم تتفتكر

**1. Auth: JWT + Guest-Id fallback (مش استبدال، الاتنين شغالين مع بعض):**
- `ICurrentUserService.CustomerId` بيقرا من الـ JWT `sub` claim لو موجود ومصدّق، ولو مفيش يرجع لـ `X-Guest-Id` header زي الأول بالظبط. مفيش `[Authorize]` على أي endpoint، فالـ resolution ده بيحصل دايماً بغض النظر عن وجود token.
- **ملاحظة تقنية مهمة**: لازم `MapInboundClaims = false` في إعدادات الـ JwtBearer، لأن الـ default handler بيستبدل الـ claim type بتاع "sub" لواحد تاني قديم (`ClaimTypes.NameIdentifier`) — من غيرها الـ lookup بيفشل بصمت.
- الفرونت بيعمل generate لـ guest GUID زي الأول (`src/lib/guestId.js`)، وكل الـ API calls بتبعته — ده لسه شغال لكل guest، وبقى فيه كمان `Authorization: Bearer` بيتبعت جنبه لو المستخدم مسجل دخول (تفاصيل الـ Frontend integration في قسم Auth فوق).

**2. Register بيحوّل الـ guest Customer بدل ما يعمل واحد جديد (Option B):**
- بدل ما `RegisterAsync` يعمل `Customer` جديد، بياخد الـ guest Customer الحالي (`GetOrCreateGuestAsync(_currentUser.CustomerId)`) ويحوّله (`IsGuest = false` + باقي البيانات) — نفس الـ `CustomerId`، فالـ cart/wishlist/addresses بتاعته بتفضل معاه تلقائي من غير أي دمج.
- لو نفس الـ guest id حاول يسجل تاني وهو بقى مسجل بالفعل، بيترفض برسالة واضحة ("هذا الحساب مسجل بالفعل").
- دمج سلة guest منفصلة وقت **login** (مش register) — ده Option C، اتأجل، تفاصيله فوق في "Auth".

**3. Admin API — كان مؤجل بالكامل، بدأ فعلياً دلوقتي (2026-08-13):**
- بناءً على طلب المستخدم زمان، الأولوية كانت لاستبدال الـ mock data في الـ Storefront الأول قبل أي حاجة تانية — ده حصل، وكل الـ storefront modules خلصوا.
- بعد كده، الأولوية اتحولت فعلياً للـ Admin API — Modules 1-3b اتعملوا (تفاصيل كاملة في قسم "Admin API" فوق). القرارات المعمارية بتاعتها في **#9** تحت.

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

**9. Admin API: extend مش duplicate، controllers منفصلة، DTOs في نفس فولدر الـ feature:**
- Write operations اتضافت على نفس الـ services الموجودة أصلاً للـ Storefront (`IProductService` بقى فيه `CreateAsync`/`UpdateAsync`/`DeleteAsync` جنب الـ Get methods) بدل ما نعمل `IAdminProductService` منفصلة — الحماية (`[Authorize]`) على مستوى الـ controller مش الـ service.
- Controllers منفصلة تحت `/api/admin/*` (`AdminOrdersController`, `AdminProductsController`, ...) بدل ما نضيف admin actions على الـ controllers الموجودة — كده الـ routes بتاعة الـ Storefront مالهاش أي تعديل، وفريق الـ Admin frontend عندهم namespace واضح ومستقر.
- Admin DTOs جوه نفس فولدر الـ feature الموجود (`DTOs/Product/SaveProductRequest.cs`) مش فولدر `DTOs/Admin/` منفصل.
- Image storage (لسه مش مبني، Module قادم): local disk (`wwwroot/uploads`) ورا `IStorageService`، قابل للاستبدال بـ cloud blob بعدين من غير ما يكسر حاجة.

**10. EF Core gotcha متكرر: إضافة child entity لـ parent متتبّع (tracked) بيتحسب Update مش Insert:**
- اتصادفنا بيه مرتين: `OrderStatusHistory` (Module 2) و`ProductVariant` replace-on-update (Module 3). السبب: لما تضيف عنصر جديد لـ collection navigation property على entity already-tracked (جاي من `FindAsync`/`GetByIdAsync` من غير `.Include`)، EF بيحاول يحدد الحالة (Added/Modified) بناءً على قيمة الـ Guid Id — وبما إن `BaseEntity.Id` بيتحط ليه قيمة (`Guid.NewGuid()`) في الـ property initializer نفسه (مش default/empty)، EF بيغلط ويفتكرها صف موجود محتاج Update بدل Insert → `DbUpdateConcurrencyException` (0 rows affected).
- **الحل الثابت المتبع في المشروع كله دلوقتي**: أي إضافة لـ child entity على parent متتبّع لازم تتعمل مباشرة عن طريق الـ DbSet بتاعة الـ child (`Context.Set<TChild>().Add(...)`) مش عن طريق `parent.Children.Add(...)`. اتطبق كده في `IOrderRepository.AddStatusHistory`، `IProductRepository.ReplaceVariantsAsync`، و`IProductRepository.ReplaceCountryPricesAsync`. **لازم يتفتكر في أي repository method جديدة بتضيف child entities.**
- ملاحظة: مبيحصلش المشكلة دي لما الـ parent نفسه جديد بالكامل (`AddAsync` على entity مش متتبّع قبل كده) — زي `OrderService.CreateAsync` اللي بيضيف `Order` جديد مع `Items`/`StatusHistory` في نفس الوقت من غير مشكلة، لأن EF بيعامل الـ graph كله كـ Added طالما الـ root نفسه جديد.

**11. Country Pricing — القرارات الأساسية (تفاصيل كاملة في قسم "Country Pricing" فوق):**
- العملة مشتقة من الدولة تلقائياً، مش مدخلة يدوياً من الأدمن (قرار اتراجع عنه المستخدم مرة وبعدين رجع للقرار الأصلي).
- `Product.Price`/`OldPrice` الموجودين أصلاً هما الـ USD/fallback — مفيش صف "USD" منفصل في جدول الأسعار.
- Products بس دلوقتي، مش Bundles ولا Coupons.
- الدولة بتتحل مرة واحدة وتتثبّت على `Customer` (زي الـ guest-id)، مش بتتحل كل request.
- MaxMind GeoLite2 (self-hosted) هي آلية الـ geolocation، شغالة فعلياً بملف حقيقي — مفيش CDN header (زي Cloudflare) لسه لأن خطة الـ hosting لسه مش متحددة.
- Module 3b (المنتج + الأدمن + الـ resolver) اتفصلت عن Module 3c (Cart/Checkout/Order) عن قصد، مش pass واحد — الاتنين خلصوا دلوقتي.
- **Module 3c — قاعدة "الكل أو لا حاجة"**: السلة بتتسعّر بعملة الدولة المحلية بس لو كل سطر فيها (كل المنتجات، ومفيش أي باقة) عنده سعر لنفس الدولة — أي سطر واحد ناقص بيرجّع السلة كلها للدولار، عشان `Subtotal`/`Total` يفضلوا رقم بعملة واحدة صحيحة رياضياً. قرار اتاخد بعد سؤال المستخدم صراحة (مش افتراض من غير سؤال) — البديل (عرض كل سطر بعملته وتحويل العملات لحساب الإجمالي) اتأجل لأنه محتاج مصدر أسعار صرف (exchange rates) مش موجود دلوقتي.
- **`Order.Currency` حقل جديد**: بيتسجل من `cart.Currency` وقت الـ checkout مباشرة (من غير إعادة resolve). الطلبات القديمة اتعملها backfill لـ `"USD"` تلقائياً في الـ migration.

### إعدادات البيئة المحلية (Local Dev)
- **Backend**: `http://localhost:5080` (متظبط في `launchSettings.json`، بروفايل "http" الافتراضي)
- **Frontend**: `http://localhost:5173` (Vite default)
- **Database**: SQL Server LocalDB، اسم الداتابيز `LuxiraDb`، الـ connection string متخزن في User Secrets مش appsettings.json
- **CORS**: مسموح بس لـ `http://localhost:5173`
- **Coupon تجريبي متزروع في الـ seed data**: `WELCOME10` (خصم 10%)
- **Admin تجريبي متزروع في الـ seed data**: `admin@luxira.sa` / `Admin@12345` (`CustomerRole.Admin`) — بيستخدم فعلياً دلوقتي لاختبار `/api/admin/*`.
- **MaxMind GeoLite2**: ملف `GeoLite2-Country.mmdb` لازم يتحط يدوياً في `Luxira.API/App_Data/` (مش متعمله commit، `*.mmdb` في `.gitignore`) — المسار متظبط في `appsettings.Development.json` تحت `GeoIp:DatabasePath`. من غيره، كل الزوار بيرجعوا USD fallback (مفيش crash، بس تسعير الدولة مش هيشتغل).
- لو حصل port collision (الباك إند مش شغال على 5080 أو الفرونت مش شغال على 5173)، الاحتمال الأكبر إن فيه process قديم من session سابقة لسه شغال على نفس البورت ولازم يتقفل الأول
