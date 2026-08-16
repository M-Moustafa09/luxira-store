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
- **Admin Dashboard**: فريق منفصل بيبنيه، فالـ Admin API لازم يكون documented كويس وresponses مستقرة من البداية عشان مايكسرش شغلهم لاحقاً. **لسه مفيش وصول لكود الـ Dashboard ده دلوقتي** — لما الفريق يخلصه، المستخدم هيشاركه عشان ندرسه ونحلله، وبعدين نبني مع بعض خطة ربط حقيقية (endpoints، auth flow، image upload، إلخ) بناءً على شكله الفعلي — مش افتراضات من غير ما نشوفه.
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

**آخر مراجعة حالة كاملة (status review): 2026-08-16**

هذا القسم بيتحدّث باستمرار مع تقدم المشروع، عشان أي session جديد (حتى لو fresh تماماً) يقدر يكمل من غير ما يعيد اكتشاف القرارات دي من الكود. القسم ده snapshot لـ"إحنا فين دلوقتي" — مش سجل تاريخي لكل تقرير اتعمل.

### الموديولات المكتملة (Storefront) ✅
1. **Categories**
2. **Products** — Entity موحّد (`Product` + `ProductVariant`)، وده حل مشكلة id-mismatch اللي كانت موجودة في الـ mock الأصلي
3. **Cart** — مربوط بـ guest identity (تفاصيل تحت)، شامل Bundle→Cart (تفاصيل تحت)
4. **Wishlist**
5. **Bundles & Offers** — شامل حساب خصم Coupon حقيقي (مش mock)
6. **Checkout & Orders**
7. **Testimonials** (آراء العملاء في الصفحة الرئيسية) — كـ entity اسمه `Testimonial` مش `Review` (السبب تحت). **مختلف عن Module 13 "Product Reviews" تحت** — ده كان الاسم اللي محجوز أصلاً عشان مايتلخبطش مع الـ Reviews الحقيقية بتاعة كل منتج لما تتضاف.
8. **Account** — Profile، Addresses (CRUD)، Order History. التفاصيل تحت.
9. **Cart notifications** — Toast تأكيد عند الإضافة/الحذف (منتج أو Bundle)، أي مكان في الموقع.
10. **تأكيد حذف من السلة** — Bottom sheet ("هل أنتِ متأكدة...؟") قبل حذف أي منتج أو Bundle من السلة، نفس شكل bottom sheets التانية في الموقع (`ConfirmSheet.jsx`).
11. **Auth (Backend + Frontend)** — كامل، تفاصيله تحت.
12. **Live Updates (Polling)** — التخفيضات/الباقات (`/offers`) وحالة الطلب (`/track-order` + "طلباتي") بتتحدث تلقائياً من غير refresh. تفاصيل كاملة في قسم "Storefront Live Updates (Polling)" تحت.
13. **Product Reviews (نظام تعليقات/تقييمات لكل منتج)** — أي زائر (ضيف أو مسجل) يقدر يضيف تقييم (1-5 نجوم + نص) على أي منتج، ظاهر فوراً من غير موافقة أدمن مسبقة. `Product.Rating`/`ReviewsCount` بقوا أرقام حقيقية محسوبة من التقييمات الفعلية بدل القيم الوهمية الثابتة اللي كانت متزروعة. تفاصيل كاملة في قسم "Product Reviews (Comments System)" تحت.

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

### Admin API ✅ (كامل — كل الـ Modules اتعملت)
مبنية بنفس الـ Service/Repository الموجودين للـ Storefront (extend مش duplicate)، تحت `/api/admin/*`، كل controller عليه `[Authorize(Roles = "Admin")]` على مستوى الـ class. Admin تجريبي متزروع: `admin@luxira.sa` / `Admin@12345`.

1. **Module 1 — Admin auth wiring** ✅: `AdminController` (`GET /api/admin/ping`) بيتأكد إن الـ role-based auth شغال end-to-end. مفيش تعديل كان لازم على إصدار الـ JWT أو `Program.cs` — الـ role claim كان متضاف من زمان في `AuthService.IssueTokensAsync`.
2. **Module 2 — Admin Orders** ✅: `GET /api/admin/orders` (كل الطلبات لكل العملاء، paginated، فلترة بالـ status)، `GET /api/admin/orders/{id}`، `PUT /api/admin/orders/{id}/status` (بيحدّث `Order.Status` ويسجّل صف جديد في `OrderStatusHistory`، برفض إعادة نفس الحالة). **ده بيحل مشكلة "تتبع الطلب مجمّد" اللي كانت متوثقة قبل كده** — الحالة بقت فعلاً بتتقدم دلوقتي.
3. **Module 3 — Admin Products CRUD** ✅: `GET/POST/PUT/DELETE /api/admin/products` + `GET /api/admin/products/{id}` (بيعيد استخدام نفس بحث/فلترة الـ Storefront). الحذف بيرفض برسالة واضحة (مش 500) لو المنتج مستخدم في سلة/باقة حالية (FK Restrict). الـ Variants بتتستبدل بالكامل عند التعديل (replace-all، مش diff).
4. **Module 3b — Country Pricing (Product display)** ✅: تفاصيل كاملة تحت في قسم "Country Pricing".
5. **Module 3c — Country Pricing (Cart/Checkout/Order)** ✅: تفاصيل كاملة تحت في قسم "Country Pricing".
6. **Module 4 — Admin Categories/Brands CRUD** ✅: `GET/POST/PUT/DELETE /api/admin/categories` (الأقسام الفرعية بتتستبدل بالكامل عند التعديل، replace-all زي الـ Variants) و`GET/POST/PUT/DELETE /api/admin/brands`. الحذف بيرفض برسالة واضحة (مش 500) لو التصنيف/العلامة مستخدمة في منتجات حالية (FK Restrict) — نفس pattern حذف المنتجات.
7. **Module 5 — Admin Image Upload** ✅: تفاصيل كاملة في قسم "Admin Image Upload" تحت.
8. **Module 6 — Admin Coupons/Bundles/Campaigns/Testimonials CRUD** ✅: تفاصيل كاملة في قسم "Admin Coupons/Bundles/Campaigns/Testimonials" تحت.

### الموديولات المتبقية
- **✅ Admin API — كامل**: كل الـ Modules (Auth wiring, Orders, Products, Country Pricing, Categories/Brands, Image Upload, Coupons/Bundles/Campaigns/Testimonials) اتعملت. مفيش Admin CRUD module متبقي.
- **❌ Payment Gateway** — لسه معلق تماماً، منتظر قرار المدير. مفيش أي `IPaymentGateway` interface أو أي كود دفع لسه (اتأكد بالبحث) — الخطة إن الـ checkout الحالي بيعمل Order من غير خطوة دفع فعلية.
- **✅ Stock/Inventory — اتحل بالكامل**: تفاصيل كاملة في قسم "Stock/Inventory" تحت.
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

### Stock/Inventory ✅ (اتحل بالكامل)
مفهوم Stock كان غايب تماماً من المشروع كله — أي عملية شراء كانت ممكن تبيع أكتر من المتاح فعلياً من غير أي تحقق. اتحل بالكامل:

- **الداتا موديل**: `Stock` (int) على `ProductVariant` (مش على `Product`) لأن الشراء بيحصل على مستوى الـ variant. اتضاف كـ field عادي في `SaveProductVariantRequest` — نفس الـ replace-all semantics الموجودة أصلاً لـ Label/ColorHex/SortOrder، مفيش admin module جديد احتاج يتعمل.
- **التحقق والخصم في `OrderService.CreateAsync`**: جوه نفس الـ transaction بتاعة إنشاء الطلب (`SaveChangesAsync` واحد = atomic). التحقق batched مش fail-on-first — كل الأسطر الناقصة بترجع مرة واحدة في رسالة الخطأ، مش أول واحد بس.
- **قرار مهم — الـ Bundles ملهاش `ProductVariantId`**: `BundleItem` بيربط بـ `Product` بس، مش variant محدد (بقرار سابق، الباقة ملهاش درجة معينة). فلما بندي bundle عن Stock، بنستخدم أول variant (أقل `SortOrder`) للمنتج — **نفس الـ "default variant" heuristic اللي `CartService.AddItemAsync` بيستخدمها أصلاً** لما حد يضيف منتج للسلة من غير ما يحدد درجة. قرار متسق مع pattern موجود، مش حاجة جديدة اتخترعت.
- **الـ Storefront**: `ProductListItemDto.InStock` (بيرجع true لو أي variant فيه Stock) بيتحكم في badge "نفذت الكمية" + تعطيل زرار "أضف إلى السلة" على كل الكروت اللي فيها زرار إضافة فعلي (`ProductCard`, `ProductGridCard`, `ProductCardOffers`, `BestSellerCard`). في صفحة تفاصيل المنتج، `ShadeSelector` بيعتّم ويمنع اختيار درجة نفذت كميتها، و`ProductActions` بيمنع "أضف إلى السلة"/"اشتري الآن" لو الدرجة المختارة نفذت.
- **Seed data**: الـ variants المتزروعة اتحطلها `Stock = 50` (كانت هتبقى 0 افتراضياً من غيرها) — عشان قاعدة بيانات تطوير جديدة متبقاش كل المنتجات فيها "نفذت الكمية" من أول تشغيل. **ده مختلف عن الإنتاج**: منتجات حقيقية جديدة هتفضل Stock = 0 لحد ما الأدمن يحدد رقم حقيقي، وده سلوك صح ومتعمد (0 هو الافتراضي الآمن).
- **اتعمله اختبار كامل end-to-end**: طلب بكمية أكبر من المتاح اترفض برسالة واضحة لكل منتج ناقص، طلب ضمن المتاح نجح وخصم من الـ variant الصح، شراء باقة خصم من الـ default variant لمنتج مشترك بالكمية الصح بالظبط. الـ UI اتفحص live في المتصفح (badge، تعطيل الزرار، تعتيم الدرجة) عن طريق تعديل Stock مباشرة في الداتابيز مؤقتاً.
- **✅ Bug اتكشف أثناء اختبار الـ Stock work — اتحل بعد كده منفصل**: تعديل منتج عن طريق Admin Products CRUD (`PUT /api/admin/products/{id}`) كان بيرجع 500 (FK constraint، Error 547) لو أي variant بتاعه متربط بـ `CartItem` في أي سلة. اتحل عن طريق تحويل `ReplaceVariantsAsync` لـ `UpsertVariantsAsync` (تفاصيل كاملة في قسم "Admin Products — Variant Upsert Fix" تحت).

### Admin Products — Variant Upsert Fix ✅
كان `PUT /api/admin/products/{id}` بيرجع 500 (FK constraint، Error 547) لو أي variant بتاع المنتج لسه متربط بـ `CartItem` في أي سلة (حتى سلة عميل تاني) — أي تعديل غير مرتبط أصلاً بالـ variants نفسها (زي تغيير السعر أو الوصف) كان ممكن يفشل. اتحل بالكامل:

- **السبب**: `ReplaceVariantsAsync` كانت بتمسح كل الـ variants القديمة وتعمل insert جديد بالكامل على كل update (نفس pattern الـ replace-all المستخدم لـ Categories/CountryPrices)، و`CartItemConfiguration` عامل `DeleteBehavior.Restrict` على `ProductVariantId` — فمسح variant لسه مربوط بسلة بيفشل على مستوى الداتابيز.
- **الحل**: `SaveProductVariantRequest` بقى فيه `Id` (nullable Guid، بيترجع من `ProductVariantDto.Id`). `IProductRepository.ReplaceVariantsAsync` اتحولت لـ `UpsertVariantsAsync`: أي variant واصل بـ `Id` بيتطابق مع صف موجود بيتحدّث في مكانه (من غير مسح/إعادة إنشاء)، أي واحد من غير `Id` بيتضاف كـ جديد، وبس اللي مش موجود خالص في الـ list الجاي بيتمسح. فأي update عادي (متضمنش شيل variant فعلي) مبيلمسش الـ `CartItem` FK خالص.
- **الحالة اللي لسه ممكن تفشل**: حذف variant فعلاً لسه مربوط بسلة عميل — دي لسه بتضرب الـ FK، بس دلوقتي بتترجع كـ 400/ProblemDetails واضح (زي رسالة حذف المنتج نفسه في `DeleteAsync`) بدل 500 خام.
- **اتعمله اختبار end-to-end**: تحديث بكل الـ variant ids القديمة رجع 200 وسلة عميل فيها variant منهم فضلت شغالة صح؛ حذف variant مربوط بسلة فعلاً رجع 400 برسالة واضحة بدل 500؛ إضافة variant جديد (من غير Id) جنب الموجودين نجحت.

### Admin Image Upload ✅
`POST /api/admin/uploads/images` — بيرفع صورة (منتج أو درجة) ويرجع رابطها عشان يتحط مباشرة في `MainImageUrl`/`ImageUrl`. اتحل بالكامل:

- **`IStorageService`**: interface بسيط (`SaveAsync(Stream, fileName)` بيرجع الرابط العام) في الـ Application layer — من غير أي ASP.NET types (`IFormFile`) بتسرب له، عشان يفضل قابل للاستبدال بـ cloud blob storage بعدين من غير ما يكسر حاجة (زي `IPaymentGateway`). التنفيذ الحالي `LocalStorageService` بيحفظ الملفات على الديسك محلياً.
- **`IUploadService`**: بيتحقق من الامتداد (jpg/jpeg/png/webp بس) والحجم (5 ميجابايت كحد أقصى) قبل ما ينادي `IStorageService` — نفس أسلوب الـ validation-then-throw الموجود في باقي الـ Services (`ValidationException` بترجع 400/ProblemDetails برسالة عربية واضحة، مش 500).
- **التخزين الفعلي**: `App_Data/uploads` (مش `wwwroot` — نفس مكان `GeoLite2-Country.mmdb` بالظبط، الفولدر ده أصلاً مخصص لملفات مش متتبعة بالـ git). متظبط عن طريق `Storage:RootPath`/`Storage:PublicPath` في `appsettings.json`. `Program.cs` بيربط `UseStaticFiles` بنفس المسار عشان الرابط الراجع (`/uploads/{guid}.{ext}`) يبقى قابل للوصول مباشرة. الملفات المرفوعة متسجلة في `.gitignore` (`Backend/Luxira/Luxira.API/App_Data/uploads/`).
- **الاسم المخزن**: كل ملف بياخد اسم جديد (`Guid.NewGuid()` + الامتداد الأصلي) — الاسم اللي العميل بعته مبيتستخدمش كـ filename فعلي، بس بيتقرا منه الامتداد بس.
- **Scope متعمد**: endpoint واحد عام للصور (`/images`) مش endpoint منفصل لكل نوع (منتج/درجة/تصنيف) — الفرونت هو اللي بيقرر فين يحط الرابط الراجع، الـ backend مش عارف ولا محتاج يعرف سياق الاستخدام.
- **اتعمله اختبار end-to-end**: رفع PNG صحيح رجع 200 مع رابط، والرابط ده اترجع فعلاً (200، `image/png`) لما اتعمله request مباشر؛ رفع `.txt` رجع 400 برسالة عربية واضحة؛ request من غير Authorization رجع 401.

### Admin Coupons/Bundles/Campaigns/Testimonials ✅
آخر جزء متبقي من الـ Admin API — نفس أسلوب extend الموجود (مش duplicate) على الـ services الأربعة الموجودة أصلاً للـ Storefront. اتحل بالكامل:

- **Coupons** (`AdminCouponsController`، `/api/admin/coupons`): كود الكوبون بيتحط uppercase وبيتاكد إنه فريد (case-insensitive عملياً لأنه دايماً uppercase) قبل الحفظ، والخصم بالنسبة المئوية متسقّف عند 100%. الحذف من غير أي حماية FK — الـ `Cart` بيخزن `CouponCode` كـ string مباشرة مش FK حقيقي، فمفيش حاجة تتكسر لما الكوبون يتحذف.
- **Testimonials**: `ITestimonialService` اتوسّعت بـ Create/Update/Delete بدل ما تتعمل service جديدة، و`SortOrder` بقى ظاهر في `TestimonialDto` عشان الأدمن يقدر يرتبهم (كان مخفي، الـ Storefront بس كان بيستخدمه داخلياً للترتيب).
- **Campaigns**: `IPromotionsService` اتوسّعت بـ CRUD كامل. **قرار مهم**: بما إن الـ Storefront (`GetActiveCampaignAsync`) بياخد أول صف `IsActive` بس (`FirstOrDefault`)، فتفعيل حملة جديدة (`IsActive = true`) بيلغي تفعيل أي حملة تانية شغالة تلقائياً (`ICampaignRepository.ClearActiveAsync`) — نفس فكرة "افتراضي واحد بس" المطبقة أصلاً على `CustomerAddress.IsDefault`. اتعمله اختبار حقيقي: تفعيل حملة تانية أدى لإلغاء تفعيل الأولى فعلاً، والـ Storefront رجع الحملة الجديدة صح.
- **Bundles** (الجزء الأكبر): `IBundleService` اتوسّعت بـ Create/Update/Delete، وبقى فيه `BundleDetailDto` جديد خاص بالأدمن بيحتوي على تفاصيل كل منتج في الباقة (`BundleItemDto`: ProductId/ProductName/ProductImageUrl/Quantity) — نفس فكرة `ProductListItemDto` مقابل `ProductDetailDto` (قايمة مختصرة مقابل تفاصيل كاملة). كل `ProductId` في عناصر الباقة بيتاكد إنه لمنتج حقيقي موجود فعلاً (`IProductRepository.GetByIdsAsync` الجديدة، batched مش N queries منفصلة) قبل الحفظ، ولو فيه منتج مش موجود بيرجع خطأ واضح بعدد المنتجات الناقصة. عناصر الباقة بتتستبدل بالكامل عند التعديل (replace-all زي الأقسام الفرعية والـ Country Prices) — **آمن هنا** لأن مفيش أي FK بيشاور على `BundleItem.Id` نفسه (على عكس `ProductVariant` اللي احتاج upsert بسبب `CartItem`). حذف باقة لسه محمي من نفس مشكلة `BundleCartItem` FK (`DeleteBehavior.Restrict`) — بيرجع 400 واضح مش 500 لو الباقة موجودة في سلة عميل.
- **اتعمله اختبار end-to-end كامل** لكل الأربعة عن طريق الـ API الشغال فعلياً: create/update/delete/list/get-by-id، حالات الرفض (نوع خصم غير صالح، نسبة خصم أكبر من 100، تقييم خارج 1-5، منتج غير موجود في باقة، باقة من غير عناصر خالص)، وسلوك تفعيل/إلغاء تفعيل الحملات.

### Storefront Live Updates (Polling) ✅
مطلب من المدير: تعديل الأدمن على خصم/كوبون أو حالة طلب لازم يوصل للعميل من غير ما يعمل refresh. اتناقشت البدائل معاه بالتفصيل الأول (SignalR/WebSockets، Polling/AJAX، SSE) قبل أي تنفيذ — القرار النهائي **Polling** (تفاصيل السبب في قرار **#15** تحت). اتحل بالكامل، Frontend بس، مفيش أي تعديل Backend:

- **`src/hooks/usePolling.js`** (جديد): hook عام بيعيد تنفيذ callback على interval ثابت، وبيوقف نفسه لما الـ tab يبقى في الخلفية (`document.visibilityState`) عشان ميضربش requests من غير داعي.
- **Scope اتحدد بعد تحليل الصفحات الفعلية الموجودة، مش افتراض**: `/offers` (بانر الحملة/الـ Campaign، الحاجة الوحيدة اللي بتعرض خصم للعميل فعلياً — مفيش صفحة "تصفح كوبونات" في الموقع أصلاً)، و`/track-order` (تتبع طلب واحد بعد الـ checkout مباشرة أو عن طريق واتساب)، و"طلباتي" في الحساب (`Account/Orders.jsx`). أي حاجة تانية (المنتجات، الباقات كـ catalog، الـ Testimonials، الـ Cart/Checkout) اتستبعدت بالاسم لأنها مش مطلوبة ومفيش فايدة حقيقية منها للعميل.
- **Intervals**: 25 ثانية لـ `/offers` و"طلباتي" (تعديل بانر أو حالة طلب متأخر كذا ثانية مالوش تأثير حقيقي على العميل)، 18 ثانية لـ `/track-order` بس (أقرب حالة "قاعد بيتابع فعلاً" في الموقع كله).
- **`/track-order` احتاج معالجة خاصة**: بعكس `/offers`/"طلباتي" اللي عندهم أصلاً fetch functions في الـ store يعاد استدعاؤها زي ما هي، الصفحة دي بتستخدم local state (مش zustand)، والـ fetch الأصلي (`handleTrack`) بيمسح الطلب المعروض ويعرض رسالة خطأ لو فشل — سلوك صح لما العميل هو اللي بيدوس بحث، بس غلط لو حصل جوه polling silent (هيمسح طلب ظاهر صح بسبب مجرد hiccup مؤقت في الشبكة). الحل: `refreshTrackedOrder` منفصلة، بتحدث الطلب بس عند النجاح ومتعملش حاجة عند الفشل (تسيب آخر حالة معروفة زي ما هي). الـ polling كمان بيستخدم آخر query نجحت فعلاً (`trackedQueryRef`) مش قيم input الحالية، عشان لو العميل بدأ يكتب رقم طلب تاني من غير ما يدوس بحث، الـ polling يفضل يحدث الطلب الصح.
- **مفيش تعديل على الـ guest-id/JWT dual auth**: `apiClient.js` بيحط الـ headers دي fresh مع كل request أصلاً، فالـ polling بيعيد استخدام نفس الآلية من غير أي كود identity جديد — نقطة أساسية في قرار اختيار Polling بدل SignalR (تفاصيل في **#15**).
- **اتعمله اختبار end-to-end حقيقي**: إنشاء Campaign عن طريق الـ admin API وهو الموقع مفتوح على `/offers` → البانر ظهر لوحده بعد poll tick واحد من غير refresh. تحديث حالة طلب عن طريق الـ admin API وهو `/track-order` مفتوح على نفس الطلب → الـ tracker اتقدم من "تم التأكيد" لـ "تم الشحن" لوحده. صفحة "طلباتي" اتعتبرت مؤكدة بنفس المنطق (مفيش اختبار منفصل ليها) لأنها بتستخدم بالظبط نفس الـ pattern (`usePolling` + إعادة نداء fetch function موجودة أصلاً في الـ store) اللي اتأكد منه مرتين فوق.

### Auth Rate Limiting + HSTS ✅
تحل بند رقم (3) من أولويات الجاهزية — الفجوتين الأمنيتين الحقيقيتين اللي كانوا موجودين بعد ما الـ Auth بقى customer-facing فعلاً. Backend بس، مفيش تعديل Frontend:

- **Rate Limiting**: `Microsoft.AspNetCore.RateLimiting` المدمجة في .NET 8 (مفيش NuGet package جديد). Policy واحدة اسمها `"auth"` (Fixed Window، 10 requests/دقيقة، `QueueLimit = 0`)، متطبقة على مستوى `AuthController` كله (`[EnableRateLimiting("auth")]`) — يعني `register`/`login`/`refresh`/`logout` كلهم بيشتركوا في نفس الـ window. التجاوز بيرجع `429 Too Many Requests` صراحة (`RejectionStatusCode`) بدل الـ `503` الافتراضي.
- **Partitioning بالـ IP مش guest-id/JWT**: قرار متعمد — الـ guest-id والـ JWT الاتنين بيتغيروا/بيتولدوا من العميل نفسه بسهولة (attacker بيقدر يغيرهم كل request)، فمفيش فايدة أمنية حقيقية منهم كمفتاح تحديد. الـ IP هو المقياس المعياري لمنع brute-force، حتى لو مش مثالي (ممكن يتشارك ورا NAT/proxy) — كافي لمستوى الحماية المطلوب دلوقتي.
- **HSTS**: `app.UseHsts()` بره الـ Development بس (قبل `UseHttpsRedirection()` مباشرة)، بالـ default الجاهز (`max-age` شهر). مفيش تخصيص إضافي — الـ defaults كافية، مفيش داعي لتعقيد زيادة.
- **اتعمله اختبار end-to-end حقيقي**: 10 requests متتالية على `/api/auth/login` عدّت (بترجع `401` لبيانات غلط، مش rate-limited)، الـ request الحادي عشر رجع `429`؛ اتأكد إن `/api/auth/register` بيشارك نفس الـ IP window (رجع `429` كمان)؛ endpoints تانية زي `/api/categories`/`/api/products` مش متأثرة خالص. الـ HSTS اتأكد منه عن طريق تشغيل السيرفر مؤقتاً بـ `ASPNETCORE_ENVIRONMENT=Production` (env vars بس، من غير أي commit) وشوهد الهيدر `Strict-Transport-Security` فعلياً على الـ HTTPS responses — الهيدر ده مش بيظهر على `localhost` العادي لأن الـ HSTS middleware في .NET بيستثني الـ loopback hosts بالـ default، ده سلوك متوقع مش مشكلة.

### Backend Unit Tests + CI ✅
تحل بند رقم (4) من أولويات الجاهزية — Deployment استُثنيت عن قصد، تفاصيل السبب تحت في قسم "سياسة الـ Merge".

- **`Luxira.Tests`** (جديد، مضاف للـ `.sln`): `xUnit` + `NSubstitute` (اختيار من ضمن الاتنين اللي CLAUDE.md سمحت بيهم، Moq/NSubstitute) + `FluentAssertions`. بيختبر الـ Service classes (اللي فيها الـ business logic الفعلية، في `Luxira.Infrastructure/Services`) ضد `IUnitOfWork`/repositories متعملهم mock — مفيش database حقيقية، ومفيش Handlers لأن المشروع مبنى بدون CQRS/MediatR أصلاً (نص "Unit tests للـ Application layer (Handlers)" الأصلي في الملف ده كان بيفترض pattern تاني، اتفسّر هنا كـ "طبقة الـ business logic" اللي فعلياً في Infrastructure).
- **21 test موزعين على 3 ملفات وقتها** (العدد الإجمالي دلوقتي 28 test بعد إضافات لاحقة — تفاصيلها موزعة في الأقسام اللي بعد كده):
  - `OrderServiceTests`: منطق حجز الـ Stock في `CreateAsync` — كل الأسطر الناقصة بترجع مع بعض (مش أول واحد بس)، الخصم الصح من الـ Stock عند النجاح، رفض سلة فاضية، واستهلاك stock الباقات من الـ default variant لكل منتج (نفس الـ heuristic الموثق في قرار #12).
  - `CouponServiceTests` + `SaveCouponRequestValidatorTests`: تحويل الكود لـ uppercase، منع تكرار الكود (شامل حالة إن العميل يحدّث نفس الكوبون بنفس كوده من غير false-positive)، وقاعدة الـ 100% cap بتتطبق بس على خصم النسبة المئوية مش المبلغ الثابت.
  - `PromotionsServiceTests`: تفعيل Campaign بيلغي تفعيل أي Campaign تانية (`excludeId: null` عند الإنشاء، `excludeId: <id الحالي>` عند التحديث)، وإلغاء التفعيل مبيلمسش حاجة تانية.
- **`.github/workflows/ci.yml`**: بيبني ويشغّل تسـتس الـ backend (Release config) وبيبني الـ frontend، على push/PR لـ `main`. كل الأوامر جوّاه اتعملها تجربة محلية أول قبل ما يتضاف الملف، عشان يشتغل صح من أول تشغيل فعلي.
- **مفيش اختبارات Frontend**: مفيش test tooling أصلاً في `lotus-blue` (لا Vitest ولا Jest)، والـ CI بيكتفي بـ `npm run build` كـ smoke check (لو فيه compile error هيتمسك) — إضافة test framework كامل للفرونت لم تُطلب ومحتاجة نقاش منفصل لو حبينا نعملها مستقبلاً.
- **مفيش Deployment**: قرار متعمد اتاخد بعد سؤال المستخدم صراحة — الـ hosting target لسه مش متحدد (زي Payment Gateway بالظبط)، فمفيش Dockerfile ولا deployment pipeline اتضاف. لما يتحدد الـ target، ده هيبقى module منفصل.

### Phase 1 — Manager Batch (Customer Blocking + Order Notifications + Visit Analytics) ✅
دفعة مهام جديدة جاية من المدير مباشرة (2026-08-15/16)، مش من أولويات الجاهزية الأصلية. اتعمل تحليل كامل لكل الـ 3 مهام الأول (خيارات، أسئلة توضيحية، تقسيم لخطوات) قبل أي كود، بالظبط زي العملية المتبعة من البداية. الأقسام الفرعية تحت.

**Task 1 — حظر عميل (Customer Blocking) ✅**
- `Customer.IsBlocked`/`BlockedAt`/`BlockedReason` (nullable) + migration. الحظر بيمنع **تسجيل الدخول** و**إنشاء طلبات جديدة** (الاتنين، بقرار من المستخدم).
- **Enforcement**: `AuthService.LoginAsync` بيتحقق من `IsBlocked` **بعد** التحقق من كلمة السر (مش قبل) — عشان محاولة تخمين كلمة سر غلط ما تكشفش إن الحساب محظور أصلاً. `OrderService.CreateAsync` بيتحقق منه أول حاجة (قبل ما يلمس الـ cart خالص)، وبيقرا حالة الحظر **من الداتابيز مباشرة** مش من الـ JWT claims — يعني الحظر بيتفعّل فوراً حتى لو العميل معاه access token لسه صالح اتصدر قبل الحظر.
- **الوصول من الأدمن**: `PUT /api/admin/orders/{id}/block-customer` — الحظر متاح من خلال أي طلب للعميل ده (مفيش Admin Customers list منفصلة، بقرار متعمد، لأن مفيش طلب صريح لصفحة إدارة عملاء كاملة، ده كان هيبقى توسع زيادة عن المطلوب). `OrderDto` بقى فيه `CustomerId`/`CustomerIsBlocked` عشان الأدمن يشوف حالة الحظر من غير call إضافي.
- **حدود معروفة ومقبولة (اتقالت للمستخدم صراحة، مش مخفية)**: الحظر قوي 100% للعميل **المسجل** (login/email عنوان ثابت، والـ email uniqueness بيمنعه يسجل تاني بنفس الإيميل). لكنه **best-effort بس** لضيف لسه ما سجلش — أي حد يقدر يمسح localStorage ويجيب guest-id جديد غير محظور، لأن مفيش أي device fingerprinting في المشروع (وقرار متعمد إننا مانضيفوش، تجنباً لتعقيد زيادة ومشاكل false-positive زي الـ IP المشترك). `POST /api/auth/refresh` **مش متأثر بالحظر عمداً** — نطاق الحظر المتفق عليه كان "login + checkout" بس، فعميل محظور معاه refresh token لسه صالح يقدر يفضل "مسجل دخول" (يشوف بروفايله بس، مش يشتري) لحد ما الـ refresh token ينتهي.
- **Frontend**: `Checkout.jsx` كان بيعرض رسالة عامة ثابتة عند أي خطأ ("تعذر إتمام الطلب")، اتصلح عشان يعرض رسالة الـ backend الحقيقية (نفس الباترن الموجود أصلاً في `Auth.jsx`) — من غيره عميل محظور كان هيشوف رسالة مالهاش معنى بدل السبب الحقيقي.
- **اتعمله اختبار end-to-end حي**: تسجيل حساب حقيقي → طلب ناجح → الأدمن يحظر → login يترفض برسالة الحظر → checkout يترفض حتى بالـ token القديم الصالح → كلمة سر غلط على حساب محظور لسه بترجع الرسالة العامة (مفيش تسريب) → إلغاء الحظر يرجّع تسجيل الدخول يشتغل → التأكد من رسالة الخطأ الحقيقية ظاهرة فعلياً على صفحة Checkout في المتصفح.

**Task 2 — إيميل تأكيد الطلب + إشعار الأدمن ✅**
- **رحلة اختيار الـ email provider**: البداية كانت Gmail SMTP مباشرة على `luxiraholding@gmail.com` (الوجهة نفسها) — اتراجعنا عنها لما اتضح إن المستخدم معندوش login access للحساب ده أصلاً (حساب الشركة مش الشخصي)، وGmail SMTP محتاج App Password على **نفس** الحساب اللي بيبعت منه. بعد بحث مقارن (Brevo/SendGrid/Mailgun/Resend، بنفس أسلوب مقارنة SignalR-vs-Polling)، الاختيار وقع على **Brevo**: free tier من غير credit card، الـ "Single Sender Verification" بتاعته محتاجة إثبات ملكية إيميل واحد **بتاعنا احنا** بس (مش الوجهة `luxiraholding@gmail.com` خالص)، وعنده SMTP relay قياسي فخلى كود MailKit اللي اتكتب أصلاً لـ Gmail يفضل شغال زي ما هو تقريباً (بس تغيير host/credentials). الدرس الأساسي: بعت **لـ** `luxiraholding@gmail.com` عمره ما احتاج أي وصول لصندوقه — الوصول كان مطلوب بس لو بعتنا **من خلاله**.
- **`IEmailService`/`SmtpEmailService`**: زي `IGeoIpLookup`، أي فشل بيتسجل في الـ log ويتبلع، مش بيرمي exception — إيميل فاشل ماينفعش يوقف عملية الـ checkout.
- **`AdminNotification`**: entity جديد، snapshot مأخوذ وقت الإنشاء (OrderNumber/CustomerName/OrderTotal/OrderCurrency) بدل FK حي — نفس فكرة `OrderItem.ProductName` — عشان الداشبورد يعرض الإشعار كامل من غير call إضافي. `Type` عبارة عن enum من أول يوم (بس `OrderConfirmed` موجود حالياً) عشان نوع إشعار جديد مستقبلاً (زي "عميل اتحظر" أو "مخزون قليل") ميحتاجش breaking change في الـ contract.
- **الـ contract اتحدد بعناية بقصد** (بناءً على تنبيه صريح من المستخدم إن فريق الداشبورد قرّب يخلص وده مش وقت نغير فيه الـ contract بسهولة بعدين): `GET /api/admin/notifications` (paginated، نفس شكل `PagedResult<T>` المستخدم في كل مكان تاني)، `GET /api/admin/notifications/unread-count` (خفيف، للـ badge polling من غير ما تجيب القايمة كاملة كل مرة)، mark-single-read و mark-all-read (الاتنين، مش واحد بس).
- **Wiring في `OrderService.CreateAsync`**: الإشعار بيتحط (staged) قبل الـ `SaveChangesAsync` بتاع الطلب نفسه عشان يتسجلوا مع بعض atomically في نفس الـ transaction، والإيميل بيتبعت **بعد** ما الحفظ ينجح بس (مش قبل) — عشان فشل في حفظ الطلب ما يبعتش إيميل عن طلب مالوش وجود فعلي.
- **اتعمله اختبار end-to-end حي بالكامل**: SMTP send حقيقي عن طريق script تجريبي منفصل (اتمسح بعدين، مكانش جزء من الكود خالص) أكد نجاح الاتصال/الـ auth/الإرسال مع Brevo فعلياً؛ طلب حقيقي عن طريق الـ API أنتج صف `AdminNotification` صحيح (رقم الطلب/اسم العميل/الإجمالي/العملة مطابقين) ومفيش أي error اتسجل أثناء إرسال الإيميل.

**Task 3 — عداد زيارات المتجر ✅**
- `SiteVisit` (`Id`, `CustomerId`, `CreatedAt`) — صف واحد لكل زيارة بدل counter رقم واحد، عشان الإجمالي والزوار الفريدون والفترات (يوم/أسبوع/شهر) كلهم يبقوا مجرد queries مختلفة على نفس الجدول من غير أي إعادة تصميم لاحقاً.
- **الزوار الفريدون رخيصين هنا تحديداً**: `CustomerId` بيعيد استخدام نفس الـ guest-id/JWT identity الموجودة أصلاً على كل request (`ICurrentUserService`) — مفيش أي device fingerprinting جديد اتضاف، الهوية المستقرة دي كانت موجودة أصلاً في المشروع.
- **`POST /api/analytics/visit`**: storefront-facing، مفيش `[Authorize]` (زي أي endpoint عام تاني).
- **الفرونت (`useTrackVisit` hook)**: بيسجل **مرة واحدة لكل جلسة متصفح (tab session)** عن طريق `sessionStorage` guard — **مش مرة لكل page load**. لو كان لكل page load، ده كان هيبقى فعلياً عداد page views مش عداد "زيارات" بالمعنى اللي الداشبورد المفروض يعرضه.
- **`GET /api/admin/analytics/visits`**: `TotalVisits`, `TotalUniqueVisitors`, `VisitsToday`, `VisitsThisWeek`, `VisitsThisMonth` كلهم في response واحد — بدل ما نختار واحد بس مقدماً، سيبنا فريق الداشبورد يختار هما عايزين يعرضوا إيه من غير ما يحتاجوا يرجعوا لينا. الفترات دي **حدود تقويمية حقيقية** (منتصف الليل UTC، آخر يوم اثنين، أول يوم في الشهر) مش نوافذ متحركة (rolling 24h/7d/30d) — كده "الأسبوع ده" بيتفق مع المعنى العادي على أي تقرير إداري.
- **اتعمله اختبار end-to-end حي**: الـ hook اتأكد إنه بيسجل مرة واحدة بالظبط لكل جلسة (تصفير الجلسة عن طريق `sessionStorage.clear()` سجل زيارة جديدة صح، وتنقل بين صفحات من غير تصفير الجلسة سجل صفر زيارات إضافية)؛ الـ endpoint الإداري اتعمله زرع بيانات يدوي بفواصل زمنية معروفة (اليوم، قبل 3 أيام، قبل 10 أيام، قبل 40 يوم) واترجعت كل الأرقام مطابقة تماماً للحساب اليدوي.

**درس إضافي من نفس الدفعة — الـ CI الأول فشل بسبب غياب `MailKit` من الـ commit**: الكود اللي بيستخدم `MailKit`/`MimeKit` (`SmtpEmailService.cs`) اتعمله commit، بس الـ `<PackageReference>` بتاعته في `Luxira.Infrastructure.csproj` مكانش — لأن الملف ده كان بيتشال عمداً من كل commit طول الجلسة عشان فيه سطرين فاضيين غير مرتبطين من قبل كده. الحل: `dotnet build --configuration Release` من نسخة نضيفة (بعد `git stash`) قبل أي إصلاح، عشان نتأكد من السبب أولاً بدل ما نخمن. تفاصيل كاملة في قرار **#21** تحت.

### Product Reviews (Comments System) ✅
دفعة تانية جاية من المدير مباشرة (Phase 2، 2026-08-16)، 3 مهام: (1) السماح للزوار (شامل الضيوف) بإضافة تعليقات/تقييمات على المنتجات، (2) الأدمن يقدر يحذف أي تعليق يدوياً، (3) الأدمن يقدر يخفي/يظهر تعليق من غير حذفه. اتعمل نفس عملية التحليل الكاملة (فهم + أسئلة توضيحية + خطوات) قبل أي كود، وكل خطوة اتعملها مراجعة حية في المتصفح قبل الانتقال للي بعدها.

- **القرارات اللي اترجعت من المستخدم على الأسئلة التوضيحية**: تقييم بالنجوم (1-5) مع النص — عشان نحسب `Product.Rating`/`ReviewsCount` من بيانات حقيقية بدل القيم الثابتة اللي كانت متزروعة. ظهور فوري من غير طابور موافقة (نفس التوصية) — القرار قابل بأن المحتوى المسيء يفضل ظاهر لحد ما الأدمن يتصرف. Products بس، مفيش entity تانية محتاجة تعليقات دلوقتي.
- **الداتا موديل**: `Review` entity جديد (`ProductId`, `CustomerId`, `AuthorName`, `Rating` (1-5)، `Text`, `IsVisible` (افتراضي `true`), `CreatedAt`). `ReviewConfiguration`: FK على `Product` بـ`DeleteBehavior.Cascade` (مختلف عن باقي الـ FKs في المشروع اللي بتستخدم `Restrict` — هنا مقبول لأن مفيش حاجة بتشاور على `Review.Id` نفسه، فحذف منتج ومسح تقييماته معاه منطقي ومالوش تأثير جانبي)، indexes على `ProductId`/`IsVisible`.
- **الـ Aggregate بتاع Product.Rating/ReviewsCount — مبني على إعادة الحساب بعد كل تعديل، مش تحديث تراكمي (running average)**: أي عملية (create/hide/show/delete) بتنادي `RecomputeProductAggregateAsync` بعد ما التعديل نفسه يتحفظ (`SaveChangesAsync` منفصلة) — بيقرا `IReviewRepository.GetVisibleStatsAsync` (count + average للتقييمات الظاهرة بس) من الداتابيز فريش وبيحدّث `Product.Rating`/`ReviewsCount` بيها مباشرة. اتقرر كده بدل حساب رياضي تراكمي (زي "لو كان متوسط X على N، والتقييم الجديد Y، المتوسط الجديد يبقى...") لأنه أبسط بكتير يتأكد من صحته لكل الحالات الأربعة (إضافة/حذف/إخفاء/إظهار) مع بعض، وأداؤه كافي هنا لأن عدد التقييمات لكل منتج مش متوقع يكون ضخم.
- **ترتيب مهم اتراعى بقصد**: الـ stats query لازم يحصل **بعد** ما التعديل بتاعه (الإضافة/الحذف/التغيير) يتحفظ في الداتابيز فعلاً، مش قبله — لو حصل قبل كده، التعديل الجديد (لسه staged بس مش persisted) مش هيظهر في نتيجة الـ query ويطلع رقم غلط بواحد. ده كان اتراجع عنه في مرحلة التصميم قبل ما يتكتب أي كود (مش bug اتصلح بعدين).
- **`GetVisibleStatsAsync` بيتفادى `AverageAsync` على قايمة فاضية**: `.AverageAsync()` في EF Core بيرمي exception لو صفر صفوف، فالـ repository بيجيب الـ ratings كـ list الأول (`.Select(r => r.Rating).ToListAsync()`) وبيتاكد إن `Count > 0` قبل ما يحسب `.Average()` — لو صفر تقييمات ظاهرة، الـ `Product.Rating` بيترجع لـ 0 صراحة مش يفضل قيمة قديمة stale.
- **EF Core fixup ملحوظ (سلوك إيجابي مش قرار)**: لما `Product` (من `GetByIdAsync`) و`Review` الجديد (بنفس الـ `ProductId`) يبقوا الاتنين متتبّعين في نفس الـ `DbContext` في نفس الـ request، EF بيملأ `review.Product` تلقائياً في الذاكرة من غير أي `.Include()` صريح — استفدنا منه إن الـ response بتاع الإنشاء (`POST /api/products/{id}/reviews`) بيرجع `ProductName`/`ProductImageUrl` حقيقيين فوراً. الـ list queries (اللي بتستخدم `AsNoTracking()`) بترجعهم `null` صح لأن الـ fixup ده محتاج tracking أصلاً.
- **Endpoints**:
  - Storefront: `GET /api/products/{id}/reviews` (paginated، ظاهر بس)، `POST /api/products/{id}/reviews` (`201 Created`) — مفيش `[Authorize]`، نفس منطق guest-id/JWT resolution المستخدم في كل مكان تاني.
  - Admin (`AdminReviewsController`، `[Authorize(Roles="Admin")]`): `GET /api/admin/reviews` (قايمة عامة لكل التقييمات لكل المنتجات، ظاهرة ومخفية، `.Include(Product)` عشان الأدمن يعرف التقييم ده لأي منتج من غير ما يفتح كل منتج لوحده)، `PUT {id}/visibility` (إخفاء/إظهار)، `DELETE {id}`.
- **Frontend**: `ProductReviews.jsx` component جديد على صفحة تفاصيل المنتج — star picker + فورم (اسم/تعليق) + قايمة التقييمات الظاهرة. بعد الإرسال الناجح، `onReviewAdded` callback بينادي `refetchProduct()` في `ProductDetails.jsx` عشان بادج التقييم في الـ hero يتحدث فوراً من غير ما العميل يعمل refresh يدوي.
- **اتعمله اختبار end-to-end حي بالكامل عن طريق الـ UI الفعلي (مش curl مباشر)**: تقييم حقيقي اتبعت من فورم الصفحة (تعبئة الحقول عن طريق JS native-setter بعد ما الكليك الإحداثي أخطأ مرتين، تفاصيل في ملاحظة أسفل)، `POST` رجع `201`، القايمة اتحدثت لوحدها (`GET` بعده رجع `200`)، والـ hero rating badge اتغير من القيمة الافتراضية الفارغة لـ "5 ★★★★★ (1)" فوراً من غير refresh. القيم اتحسبت يدوياً وقورنت بالنتائج الفعلية عبر دورة كاملة (إضافة → إخفاء → إظهار → حذف) وطابقت بالظبط.
- **ملاحظة تشغيلية (مش قرار معماري)**: الكليك الإحداثي (coordinate-based) في اختبار المتصفح أخطأ مرتين أثناء تعبئة الفورم (مرة فاضل التكست إريا فاضي، ومرة نقل لصفحة منتج تاني بالغلط عن طريق كليك على related-product card) — اتحل بالتحول لأسلوب JS native-setter (`Object.getOwnPropertyDescriptor(...).set` + `dispatchEvent`) لتعبئة الحقول، واستخدام `find` للحصول على element ref فريش للزرار بدل إحداثيات قديمة من screenshot سابق. درس عملي لاختبارات المتصفح المستقبلية، مش تغيير في الكود.

### Production-Readiness — الحالة الفعلية بعد المراجعة
| البند | الحالة |
|---|---|
| Rate Limiting على `/auth/*` | ✅ اتحل — 10 requests/دقيقة لكل IP، 429 عند التجاوز. تفاصيل في قسم "Auth Rate Limiting + HSTS" تحت |
| Structured Logging (Serilog) | 🟡 جزئي — Serilog شغال، بس Console sink بس، مفيش persistence لأي مكان تاني |
| Unit Tests | ✅ اتحل — 21 test في `Luxira.Tests` (Service classes + validators). تفاصيل في قسم "Backend Unit Tests + CI" تحت |
| HTTPS / HSTS | ✅ اتحل — `UseHsts()` شغال بره الـ Development. تفاصيل في قسم "Auth Rate Limiting + HSTS" تحت |
| CORS | 🟡 dev-only — `http://localhost:5173` بس، مفيش domain إنتاج لسه (متوقع، مفيش إنتاج لسه) |
| CI (build + test gate) | ✅ اتحل — `.github/workflows/ci.yml`. تفاصيل في قسم "Backend Unit Tests + CI" تحت |
| Deployment | ❌ لسه معلق — منتظر قرار hosting target (زي Payment Gateway بالظبط)، لا Dockerfile ولا pipeline نشر |

### الأولوية المقترحة لجاهزية المتجر لعملاء حقيقيين
بالترتيب من الأكتر حرجاً: **(1)** ~~Admin API~~ **اتعمل ✅ بالكامل** (Auth wiring + Orders + Products + Country Pricing + Categories/Brands + Image Upload + Coupons/Bundles/Campaigns/Testimonials — مفيش Module متبقي) → **(2)** Payment Gateway (الـ checkout مش بياخد فلوس فعلياً، منتظر قرار المدير) → **(3)** ~~Rate Limiting على `/auth/*` + HSTS~~ **اتعمل ✅** → **(4)** ~~Tests + CI~~ **اتعمل ✅** (Deployment استُثنيت عن قصد، تفاصيل تحت). ~~ربط الـ Auth بالفرونت~~ **اتعمل ✅**. ~~تتبع الطلب مجمّد~~ **اتعمل ✅** (عن طريق Admin Orders). ~~Module 3c: Cart/Checkout/Order يستخدموا سعر الدولة~~ **اتعمل ✅**. ~~Stock/Inventory~~ **اتعمل ✅**. ~~Image Upload~~ **اتعمل ✅**. ~~Storefront Live Updates (Polling)~~ **اتعمل ✅** (طلب من المدير، مش من ضمن الترتيب الأصلي). ~~Phase 1 Manager Batch (Customer Blocking + Order Notifications + Visit Analytics)~~ **اتعمل ✅** (طلب من المدير كمان، مش من ضمن الترتيب الأصلي — تفاصيل في قسم "Phase 1 — Manager Batch" فوق). ~~Phase 2: Product Reviews (Comments System)~~ **اتعمل ✅** (طلب من المدير كمان، تفاصيل في قسم "Product Reviews (Comments System)" فوق). **البنود الوحيدة المتبقية دلوقتي من الأولويات الأصلية**: **(2) Payment Gateway** و**Deployment** — الاتنين معلقين بانتظار قرارات من المستخدم (اختيار الـ gateway، واختيار الـ hosting target)، مفيش حاجة تانية نقدر ننفذها فيهم من غير المستخدم. اللغة (Task 2) وOption C (guest cart merge on login) أقل حرجاً من دول التاني — مفيش منهم بيمنع عميل يتصفح/يسجل/يشتري.

### حالة الـ Branches
- `main` — up to date مع origin لحد آخر push (commit `94ca4a4`، Phase 2: Product Reviews (Comments System)، اتعمله merge عن طريق GitHub PR #10، وCI اتأكد إنه نجح فعلياً على الـ merge commit نفسه). كل الـ storefront modules، Bundle→Cart، Cart notifications، تأكيد حذف من السلة، Auth backend + frontend، Admin API كامل، Stock/Inventory، Variant Upsert Fix، Live Updates Polling، Rate Limiting + HSTS، Unit Tests + CI، Phase 1 Manager Batch (Customer Blocking + Order Notifications + Visit Analytics)، Phase 2 Product Reviews — كلهم متعملهم merge وموجودين ومدفوعين لـ origin.
- كل الـ feature branches السابقة (`feature/auth`, `feature/cart-notifications`, `feature/cart-remove-confirm`, `feature/auth-frontend`, `feature/bundle-to-cart`, `feature/admin-api-country-pricing`, `feature/admin-categories-brands`, `feature/stock-inventory`, `fix/product-variant-update-fk`, `feature/admin-image-upload`, `feature/admin-coupons-bundles-campaigns-testimonials`, `feature/storefront-live-updates-polling`, `feature/auth-rate-limiting-hsts`, `feature/backend-unit-tests-ci`, `feature/customer-blocking`, `feature/order-email-and-notifications`, `feature/site-visit-analytics`, `feature/product-reviews`, `docs/status-update`, `docs/auth-status-update`, `docs/stock-status-update`, `docs/variant-fk-fix-status-update`, `docs/image-upload-status-update`, `docs/admin-crud-status-update`, `docs/live-updates-polling-status-update`, `docs/rate-limiting-hsts-status-update`, `docs/tests-ci-status-update`, `docs/phase1-manager-batch-status-update`) اتعملها merge بالكامل لـ `main` ومفيش commits قدامها. مفيش شغل معلق على branch منفصل دلوقتي (غير الـ `docs/phase2-reviews-status-update` الحالي بتاع التحديث ده نفسه).

### سياسة الـ Merge: PR-based دلوقتي (اتحوّلت من محلي بعد ما CI اتضاف)
- **السياسة الحالية**: كل feature بتاخد branch منفصل، وبعد المراجعة في الـ chat بيتعمل `git push` للـ branch وفتح **GitHub PR** — مش `git merge --no-ff` محلي زي الأول. الـ CI (`.github/workflows/ci.yml`) بيشتغل تلقائي على الـ PR، والمستخدم هو اللي بيعمل الـ merge على GitHub بنفسه.
- **التحول ده حصل فعلياً مع أول PR**: `feature/backend-unit-tests-ci` (PR #4) — أول branch اتعمله push + PR + merge عن طريق GitHub بدل الطريقة المحلية، بالظبط زي ما كان متوقع في القرار الأصلي.
- **السبب في التحول**: نفس القرار الموثق قبل كده — إن الـ PR-based workflow يبقى منطقي لما يتضاف CI حقيقي (بيشتغل تلقائي على الـ PR) بدل ما يبقى خطوة زيادة من غير فايدة فعلية.
- **حدود الـ agent الحالية**: الـ `gh` CLI مش متاح في بيئة التنفيذ دلوقتي، فـ push + فتح الـ PR بيحصلوا عن طريق الـ agent، بس **الـ merge الفعلي على GitHub لازم يحصل من المستخدم نفسه** — الـ agent بيأكد إن الـ merge حصل صح بعد كده عن طريق `git fetch`/`pull --ff-only` على `main`.
- **درس اتعلمناه من حادثة PR #7 (تفاصيل كاملة في قرار #21)**: مجرد إن الـ build نجح محلياً مش ضمان كافي — الـ agent دلوقتي بيتأكد من نتيجة الـ CI run الفعلية على GitHub (عن طريق الـ Actions API، حتى من غير `gh`/token، الـ API الأساسي شغال من غير auth للـ repos العامة) قبل ما يقول إن أي PR جاهز للـ merge، وبعد الـ merge كمان بيتأكد إن الـ run على الـ merge commit نفسه نجح — مش بس إن المستخدم قال "اتعمله merge".

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
- Image storage: local disk (`App_Data/uploads`، مش `wwwroot` — تفاصيل كاملة في قسم "Admin Image Upload" وقرار **#13**) ورا `IStorageService`، قابل للاستبدال بـ cloud blob بعدين من غير ما يكسر حاجة.

**10. EF Core gotcha متكرر: إضافة child entity لـ parent متتبّع (tracked) بيتحسب Update مش Insert:**
- اتصادفنا بيه مرتين: `OrderStatusHistory` (Module 2) و`ProductVariant` replace-on-update (Module 3). السبب: لما تضيف عنصر جديد لـ collection navigation property على entity already-tracked (جاي من `FindAsync`/`GetByIdAsync` من غير `.Include`)، EF بيحاول يحدد الحالة (Added/Modified) بناءً على قيمة الـ Guid Id — وبما إن `BaseEntity.Id` بيتحط ليه قيمة (`Guid.NewGuid()`) في الـ property initializer نفسه (مش default/empty)، EF بيغلط ويفتكرها صف موجود محتاج Update بدل Insert → `DbUpdateConcurrencyException` (0 rows affected).
- **الحل الثابت المتبع في المشروع كله دلوقتي**: أي إضافة لـ child entity على parent متتبّع لازم تتعمل مباشرة عن طريق الـ DbSet بتاعة الـ child (`Context.Set<TChild>().Add(...)`) مش عن طريق `parent.Children.Add(...)`. اتطبق كده في `IOrderRepository.AddStatusHistory`، `IProductRepository.UpsertVariantsAsync` (كان اسمه `ReplaceVariantsAsync` — اتغيّر بعد fix منفصل، تفاصيل تحت)، و`IProductRepository.ReplaceCountryPricesAsync`. **لازم يتفتكر في أي repository method جديدة بتضيف child entities.**
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

**12. Stock/Inventory — الباقات بتستهلك من الـ default variant (تفاصيل كاملة في قسم "Stock/Inventory" فوق):**
- `BundleItem` ملهوش `ProductVariantId` (الباقة ملهاش درجة معينة)، فاستهلاك الـ Stock بتاعها بيتحسب على أول variant (أقل `SortOrder`) لكل منتج — نفس الـ heuristic اللي `CartService.AddItemAsync` بيستخدمه أصلاً لما حد يضيف منتج من غير ما يحدد درجة، مش قرار جديد منفصل.
- التحقق من الـ Stock وخصمه بيحصلوا جوه نفس transaction إنشاء الطلب (`OrderService.CreateAsync`)، والتحقق batched (كل الأسطر الناقصة في رسالة واحدة) مش fail-on-first.
- Seed data اتحطلها `Stock = 50` لكل variant عشان قاعدة بيانات تطوير جديدة تفضل قابلة للاستخدام — القرار ده خاص بالـ seed بس، منتجات إنتاج حقيقية جديدة لازم تفضل `Stock = 0` لحد ما الأدمن يحدد رقم حقيقي (الافتراضي الآمن).
- **✅ اتكشف bug موجود من قبل أثناء الاختبار — اتحل بعد كده في fix منفصل**: تعديل منتج (`PUT /api/admin/products/{id}`) كان بيرجع 500 لو أي variant بتاعه لسه مربوط بـ `CartItem` في أي سلة، بسبب `ReplaceVariantsAsync` (مسح+إعادة إنشاء) مع `DeleteBehavior.Restrict` على `CartItem.ProductVariantId`. تفاصيل الحل في قسم "Admin Products — Variant Upsert Fix" تحت.

**13. Admin Image Upload — تخزين على الديسك في `App_Data`، مش `wwwroot`:**
- القرار الأصلي (في **#9**) كان `wwwroot/uploads`، بس التنفيذ الفعلي استخدم `App_Data/uploads` بدل منه — نفس المكان اللي فيه `GeoLite2-Country.mmdb` بالظبط، عشان الفولدر ده أصلاً مخصص لملفات مش متتبعة بالـ git ومش جزء من الكود، فمفيش داعي لـ `wwwroot` convention لمشروع API-only (مفيش static frontend اتقدم من نفس السيرفر). الرابط العام لسه شغال زي المتوقع عن طريق `UseStaticFiles` بيربط `/uploads` بنفس المسار ده — الفرق مكاني بس، مفيش تغيير في الشكل اللي الفرونت هيشوفه.
- `IStorageService` اتصمم بـ `Stream`/`fileName` بس (من غير `IFormFile`) عشان الـ Application layer يفضل مالوش أي dependency على ASP.NET Core — الـ Controller (في الـ API layer) هو اللي بيحول `IFormFile` لـ stream قبل ما ينادي الـ service.
- Endpoint واحد عام (`/api/admin/uploads/images`) مش endpoint لكل نوع صورة (منتج/درجة) — قرار متعمد لتبسيط الـ API، الفرونت بيحدد فين يحط الرابط الراجع.

**14. Admin Coupons/Bundles/Campaigns/Testimonials — نفس pattern الـ extend، مفيش حاجة جديدة معمارياً:**
- الأربعة اتبنوا على نفس الـ services الموجودة أصلاً للـ Storefront (زي قرار **#9**) — مفيش `IAdminCouponService` ولا أي اسم منفصل.
- Campaign "افتراضي واحد بس شغال" (`IsActive`) هو تطبيق تاني لنفس pattern `CustomerAddress.IsDefault` — مش قرار جديد، مجرد إعادة استخدام لنفس الفكرة في سياق مختلف.
- Bundle items بتتعمل replace-all (زي SubCategories/CountryPrices) مش upsert (زي ProductVariant) — الفرق مش تحكيمي، هو نتيجة مباشرة لغياب أي FK على `BundleItem.Id` نفسه. لو حد ضاف مستقبلاً جدول بيشاور على `BundleItem` (زي ما `CartItem` بيشاور على `ProductVariant`)، لازم نرجع نراجع القرار ده ونحوله upsert.
- `IProductRepository.GetByIdsAsync` الجديدة (AsNoTracking، batched) اتضافت خصيصاً للتحقق من عناصر الباقة — أي حاجة تانية محتاجة تتاكد من مجموعة IDs موجودة فعلاً (مش واحد بواحد) تقدر تعيد استخدامها.

**15. Live Updates: Polling بدل SignalR/WebSockets (تفاصيل كاملة في قسم "Storefront Live Updates (Polling)" فوق):**
- طلب من المدير: تعديل أدمن (خصم/كوبون، حالة طلب) يوصل للعميل من غير refresh. اتعمل تحليل trade-offs كامل قبل أي تنفيذ (SignalR، Polling، SSE) وعُرض على المستخدم قبل ما يتاخد القرار — مش افتراض من غير سؤال.
- **السبب في اختيار Polling**: الحالتين المطلوبتين (بانر خصم يظهر، حالة طلب تتقدم) بيحصلوا بمعدل بطيء (تعديل أدمن يدوي) ومفيش أي احتياج حقيقي لـ sub-second delivery — تأخير 15-25 ثانية مش محسوس للعميل. كمان SignalR/SSE هيحتاجوا connection-time identity resolution جديدة تماماً فوق نظام الـ guest-id/JWT الحالي (المتصمم أصلاً لـ per-request resolution مش persistent connections)، وهيحتاجوا لسه REST fetch كـ fallback عند إعادة الاتصال (الـ delivery مش مضمونة) — يعني مش بيلغوا الحاجة لـ polling أصلاً، بيضيفوا layer فوقه. مفيش سبب حقيقي يبرر التعقيد ده هنا (مبدأ "no over-engineering" في أول الملف).
- **Scope اتحدد بعد فحص الصفحات الموجودة فعلياً**: مفيش صفحة "تصفح كوبونات" في الموقع خالص (الكوبون بيتكتب يدوي في `CouponInput` بالـ Cart وبيتحقق منه live أصلاً) — فطلب "الخصم يوصل للعميل" بيتغطى بس عن طريق بانر الـ Campaign في `/offers`. لو المدير قصده حاجة تانية (زي صفحة تعرض الكوبونات المتاحة)، ده UI feature ناقص مش polling gap، ومحتاج نقاش منفصل.
- مفيش أي تعديل Backend خالص — كل الصفحات بتستخدم GET endpoints موجودة أصلاً.

**16. Auth Rate Limiting: partitioning بالـ IP، مش بالـ guest-id/JWT (تفاصيل كاملة في قسم "Auth Rate Limiting + HSTS" فوق):**
- قرار متعمد يستحق التوثيق لأنه ممكن يتلخبط مع باقي المشروع اللي بيعتمد على guest-id/JWT كمفتاح identity في كل حتة تقريباً (`ICurrentUserService.CustomerId`). هنا مختلف: الهدف منع brute-force مش تحديد هوية العميل، والـ attacker بيقدر يغيّر guest-id/JWT كل request بسهولة تامة، فمفيش فايدة حماية حقيقية منهم كمفتاح. الـ IP هو المفتاح المعياري لسيناريو زي ده تحديداً.
- الـ policy واحدة بس على مستوى الـ controller كله (`register`/`login`/`refresh`/`logout` بيشاركوا نفس الـ window) — مفيش داعي لـ policy منفصلة لكل endpoint دلوقتي، ده كان هيبقى تعقيد زيادة عن اللزوم.

**17. Unit Tests + CI: Deployment استُثنيت عن قصد، والتحول لـ PR-based merge حصل فعلياً (تفاصيل كاملة في قسم "Backend Unit Tests + CI" وقسم "سياسة الـ Merge" فوق):**
- "Tests + CI/Deployment" كانت بند واحد في أولويات الجاهزية، بس اتفصلت عمداً بعد سؤال المستخدم — Deployment محتاجة قرار hosting target الأول (زي Payment Gateway بالظبط)، فمفيش فايدة نبني Dockerfile/pipeline نشر قبل ما نعرف نشر فين. لما القرار يتاخد، Deployment هتبقى module منفصل.
- إضافة CI حقيقي فعّلت القرار الموثق من زمان في قسم سياسة الـ Merge: التحول من `git merge --no-ff` محلي لـ GitHub PR-based workflow. أول branch اتعمله بالطريقة الجديدة كان `feature/backend-unit-tests-ci` نفسه (PR #4) — يعني أول حاجة استخدمت الـ CI هي كمان أول حاجة استخدمت الـ PR workflow اللي الـ CI ده بيخدمه.
- الـ test project (`Luxira.Tests`) بيختبر الـ Service classes (Infrastructure) مش "Application layer (Handlers)" زي ما كان مكتوب أصلاً في بداية الملف — النص القديم كان بيفترض pattern فيه Handlers (زي CQRS)، والمشروع ده معندوش. القرار اتوضح كـ تفسير لنية النص الأصلي (اختبار طبقة الـ business logic) مش تعديل فعلي في القرار.

**18. Customer Blocking — الحظر قوي للمسجلين، best-effort للضيوف، وده مقبول (تفاصيل كاملة في قسم "Phase 1 — Manager Batch" فوق):**
- قرار متعمد اتقال للمستخدم صراحة قبل التنفيذ، مش افتراض مخفي: بما إن الهوية للضيف مجرد GUID في الـ localStorage من غير أي fingerprinting، حظر ضيف لسه ما سجلش دايماً قابل للتفادي (مسح الـ storage = هوية جديدة). إضافة fingerprinting لسد الفجوة دي رُفضت بقصد — تعقيد زيادة عن اللزوم لمشكلة الحل بتاعها مليان false-positives (IP مشترك مثلاً).
- `POST /api/auth/refresh` مش متأثر بالحظر — نطاق الحظر المتفق عليه كان "login + checkout" بس صراحة، فمفيش توسع من غير سؤال.
- الوصول للحظر اتحط على `AdminOrdersController` (مش Admin Customers list جديدة) لأن الطلب الأصلي كان "إمكانية حظر عميل" بس، مش "شاشة إدارة عملاء" — بناء الأخيرة كان هيبقى توسع في النطاق من غير طلب صريح.

**19. Order Email + Notifications — Brevo بدل Gmail، والفرق بين "يبعت من" و"يبعت لـ" (تفاصيل كاملة في قسم "Phase 1 — Manager Batch" فوق):**
- التنفيذ الأول (Gmail SMTP مباشرة) اترجع عنه بعد ما اتضح إن المستخدم معندوش login access لـ `luxiraholding@gmail.com` (حساب شركة مش شخصي). الدرس: إرسال إيميل **لـ** عنوان مش بيحتاج أي وصول لصندوقه — الوصول كان مطلوب بس لأننا اخترنا نبعت **من خلاله**. اختيار Brevo (verified sender منفصل، مش الوجهة نفسها) حل المشكلة من جذرها.
- الـ contract بتاع `AdminNotification` (pagination، unread-count منفصل، mark-single/mark-all، Type enum من أول يوم) اتحدد بعناية زيادة عن المعتاد بسبب تنبيه صريح من المستخدم إن فريق الداشبورد قرّب يخلص ومش هيبقى سهل نغير الـ contract بعدين.
- الإشعار بيتحط (staged) قبل `SaveChangesAsync` بتاع الطلب (يتسجلوا مع بعض atomically)، والإيميل بيتبعت بعدها بس — نفس الترتيب المستخدم في أي مكان تاني فيه side-effect خارجي مرتبط بعملية DB.

**20. Site Visit Analytics — صف لكل زيارة بدل counter واحد، والـ identity الموجودة أصلاً كفت (تفاصيل كاملة في قسم "Phase 1 — Manager Batch" فوق):**
- القرار بين "counter واحد" و"صف لكل زيارة" اتحسم لصالح الصف لكل زيارة رغم إنه شكلياً أعقد شوية، لأنه فعلياً أبسط على المدى المتوسط — بيدي total/unique/period breakdowns كلهم من غير أي schema تاني لاحقاً.
- الزوار الفريدون رخيصين هنا تحديداً لأن الـ guest-id/JWT identity الموجودة أصلاً على كل request اتاستخدمت زي ما هي — مفيش حاجة جديدة اتضافت للمشروع عشان الميزة دي.
- الفترات (يوم/أسبوع/شهر) حدود تقويمية حقيقية (UTC) مش rolling windows — قرار واعي إن كده أقرب لمعنى "الأسبوع ده" على أي تقرير إداري عادي.
- الـ hook بيسجل مرة لكل جلسة متصفح (`sessionStorage` guard) مش لكل page load — تمييز متعمد بين "زيارة" و"page view"، لأن الاتنين مقاسين مختلفين وده كان ممكن يعطي رقم مضلل على الداشبورد.

**21. حادثة CI في PR #7 — استبعاد ملف من الـ commits بقصد كويس ممكن يسيب فجوة حقيقية (تفاصيل السبب والحل موثقة في تاريخ commit `fix(ci): commit the missing MailKit package reference`):**
- `Luxira.Infrastructure.csproj` كان فيه diff غير مرتبط (سطرين فاضيين) من قبل هذه الجلسة، فاتعمله استبعاد من كل `git add` طول الوقت عشان مانلخبطش commits بحاجة مالهاش علاقة. لما `dotnet add package MailKit` عدّل نفس الملف (إضافة `<PackageReference>` حقيقية ومطلوبة)، الاستبعاد الشامل ده مسك التعديل الحقيقي مع القديم — فالكود اللي بيستخدم MailKit اتعمله commit، بس الـ package reference نفسه لأ. محلياً الـ build فضل شغال بسبب حالة uncommitted كانت لسه موجودة، والـ CI (checkout نضيف) كشف الفجوة فوراً.
- **الدرس المتبع دلوقتي**: أي استبعاد ملف من commit بسبب diff قديم غير مرتبط لازم يتراجع عليه (`git diff <file>`) قبل كل commit تاني يلمس نفس الملف، مش يتفترض إنه لسه بس نفس الـ noise القديم. لما ظهرت المشكلة، السبب اتأكد بإعادة إنتاجه محلياً (`git stash` + build نظيف بنفس Release config اللي الـ CI بيستخدمها) قبل أي إصلاح — مش تخمين.

**22. Product Reviews — إعادة حساب الـ aggregate بعد كل تعديل بدل تحديث تراكمي، والـ FK هنا `Cascade` مش `Restrict` (تفاصيل كاملة في قسم "Product Reviews (Comments System)" فوق):**
- بخلاف كل الـ FKs التانية في المشروع (Product→Category/Brand، Order→Customer، إلخ) اللي بتستخدم `DeleteBehavior.Restrict` عشان تمنع حذف حاجة لسه مرتبطة بحاجة تانية، `Review→Product` عامل `Cascade` عن قصد — لأن مفيش أي FK بيشاور على `Review.Id` نفسه (على عكس `ProductVariant` اللي `CartItem` بيشاور عليه)، فحذف منتج ومسح تقييماته معاه مالوش أي تأثير جانبي خطير.
- `RecomputeProductAggregateAsync` بيتنادى بعد أي تعديل (create/hide/show/delete) وبيقرا الإحصائيات فريش من الداتابيز (`GetVisibleStatsAsync`) بدل ما يحاول يحدّث `Product.Rating`/`ReviewsCount` رياضياً في الذاكرة — قرار واعي لصالح البساطة وسهولة التأكد من الصحة عبر الحالات الأربعة كلها، مش تحسين أداء متعمد. لو عدد التقييمات لمنتج واحد كبر أوي مستقبلاً (آلاف)، ده المكان اللي محتاج يترجع يتراجع فيه.

### إعدادات البيئة المحلية (Local Dev)
- **Backend**: `http://localhost:5080` (متظبط في `launchSettings.json`، بروفايل "http" الافتراضي)
- **Frontend**: `http://localhost:5173` (Vite default)
- **Database**: SQL Server LocalDB، اسم الداتابيز `LuxiraDb`، الـ connection string متخزن في User Secrets مش appsettings.json
- **CORS**: مسموح بس لـ `http://localhost:5173`
- **Coupon تجريبي متزروع في الـ seed data**: `WELCOME10` (خصم 10%)
- **Admin تجريبي متزروع في الـ seed data**: `admin@luxira.sa` / `Admin@12345` (`CustomerRole.Admin`) — بيستخدم فعلياً دلوقتي لاختبار `/api/admin/*`.
- **MaxMind GeoLite2**: ملف `GeoLite2-Country.mmdb` لازم يتحط يدوياً في `Luxira.API/App_Data/` (مش متعمله commit، `*.mmdb` في `.gitignore`) — المسار متظبط في `appsettings.Development.json` تحت `GeoIp:DatabasePath`. من غيره، كل الزوار بيرجعوا USD fallback (مفيش crash، بس تسعير الدولة مش هيشتغل).
- لو حصل port collision (الباك إند مش شغال على 5080 أو الفرونت مش شغال على 5173)، الاحتمال الأكبر إن فيه process قديم من session سابقة لسه شغال على نفس البورت ولازم يتقفل الأول
