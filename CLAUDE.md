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

## حالة المشروع الحالية (Living Status — آخر تحديث: 2026-08-11)

هذا القسم بيتحدّث باستمرار مع تقدم المشروع، عشان أي session جديد (حتى لو fresh تماماً) يقدر يكمل من غير ما يعيد اكتشاف القرارات دي من الكود.

### الموديولات المكتملة (Storefront)
1. **Categories** ✅
2. **Products** ✅ — Entity موحّد (`Product` + `ProductVariant`)، وده حل مشكلة id-mismatch اللي كانت موجودة في الـ mock الأصلي (كان فيه IDs مختلفة لنفس المنتج في أكتر من ملف)
3. **Cart** ✅ — مربوط بـ guest identity (تفاصيل تحت)
4. **Wishlist** ✅
5. **Bundles & Offers** ✅ — شامل حساب خصم Coupon حقيقي (مش mock)
6. **Checkout & Orders** ✅
7. **Reviews** ✅ — كـ entity اسمه `Testimonial` مش `Review` (السبب تحت)

### فلاتر صفحة Products (اتعملت الجلسة دي) ✅
- الفلاتر الأربعة (العلامة/Brand، السعر/Price، التقييم/Rating، نوع البشرة/SkinType) اتبنت بالكامل Backend + Frontend ومتأكد منها live في المتصفح.
- **Brand**: entity حقيقي جديد (`Luxira.Domain/Entities/Brand.cs`) زي `Category` بالظبط، مش enum ولا string خام. اتعمله migration (`AddBrandsAndProductFilters`) شاملة seeding (`DbSeeder.SeedBrandsAsync`) وbackfill غير مشروط (`BackfillProductBrandsAsync`) بيربط المنتجات القديمة بالـ Brand الصح كل startup لحد ما تتظبط، من غير ما يكسر منتجات اتعمل لها ربط قبل كده.
- **Price**: نطاق حر (`MinPrice`/`MaxPrice`) مش قيم محددة مسبقاً.
- **Rating**: threshold أدنى بس (`MinRating`) مش نطاق كامل — القرار كان إن ده أبسط وكافي للاستخدام الفعلي.
- **SkinType**: `enum` nullable على `Product` (`Luxira.Domain/Entities/SkinType.cs`) مش entity/جدول منفصل — لأنه taxonomy صغير وثابت ومش هيتضاف/يتعدل من الـ Admin زي الـ Category/Brand. **قرار متعمد**: مفيش أي بيانات SkinType متزروعة للمنتجات الحالية (كلها color cosmetics مالهاش علاقة حقيقية بنوع بشرة محدد) — الفلتر شغال فعلياً (بيرجع 0 منتج دلوقتي لأي نوع بشرة، ده صح) لحد ما يبقى فيه منتجات skincare حقيقية ليها بيانات صح تتزرع.
- الـ `IProductRepository.SearchAsync` اتبنى على `ProductSearchCriteria` (Domain type) بدل ما الـ method signature يكبر أكتر من اللازم (كان وصل لـ 11 parameter).
- Frontend: `OptionsBottomSheet.jsx` (component عام حل محل `CategoryBottomSheet.jsx` القديم) بيغطي Category/Brand/Rating/SkinType كلهم بنفس الشكل، و`PriceBottomSheet.jsx` منفصل للـ range input. مفيش pattern UI جديد اتضاف.

### الموديولات المتبقية
- **Module 8: Account** (profile, addresses, order history) — لسه معملش
- **Admin API** — مؤجل بالكامل لحد دلوقتي، القرار إننا نضيفه module-by-module بعد كل storefront module بدل ما نعمله كله مرة واحدة في الآخر
- **Auth** — مؤجل، شغال حالياً بآلية guest-id مؤقتة (تفاصيل تحت)
- **Bundle → Cart** — قرار تصميم لسه مفتوح (تفاصيل تحت في القرارات المهمة)
- **بنود من تقرير الـ Production-Readiness لسه معلقة**: Payment Gateway (منتظر قرار المدير)، JWT Auth الحقيقي، Rate Limiting على `/auth/*`، Serilog structured logging، Unit tests (xUnit/Moq/FluentAssertions)، HTTPS/HSTS + CORS للإنتاج، خطة الـ Deployment/CI. كل دول اتوثقوا في roadmap منفصل (Artifact) اتعرض على المستخدم قبل كده — راجعه قبل ما تبدأ في أي بند منهم بدل ما تفترض الأولوية من نفسك.

### قرارات مهمة لازم تتفتكر

**1. Auth مؤجل + آلية Guest-Id (Seam جاهز للترقية لاحقاً):**
- مفيش JWT/تسجيل دخول حالياً. بدل كده فيه seam واحد بس اسمه `ICurrentUserService` (interface في Application layer)، وتنفيذه الوحيد دلوقتي (`Luxira.API/Services/CurrentUserService.cs`) بيقرا GUID من header اسمه `X-Guest-Id` بيتبعت مع كل request.
- الفرونت بيعمل generate لـ GUID واحد أول مرة (`crypto.randomUUID()`) ويخزّنه في `localStorage` (`src/lib/guestId.js`)، وكل الـ API calls (`apiGet`/`apiPost`/`apiPut`/`apiDelete` في `src/lib/apiClient.js`) بتبعته تلقائي.
- أول استخدام لأي guest id في عملية (Cart/Wishlist/Order) بيعمل له `Customer` row تلقائي (`IsGuest = true`, `PasswordHash = null`) عن طريق `ICustomerRepository.GetOrCreateGuestAsync`.
- **لما الـ Auth module يتعمل بعدين**: التغيير المطلوب الوحيد هو استبدال تنفيذ `CurrentUserService` عشان يقرا الـ customer id من الـ JWT claims بدل الـ header — مفيش أي تعديل مطلوب في أي Service/Controller تاني لأن الكل بيتعامل مع `ICurrentUserService.CustomerId` بس.
- كل الـ endpoints دلوقتي مفتوحة تماماً (مفيش `[Authorize]` في أي مكان) — ده مقصود ومؤقت لحد ما الـ Auth module يتعمل، مش نسيان.

**2. Admin API مؤجل بالكامل:**
- بناءً على طلب المستخدم، الأولوية كانت لاستبدال الـ mock data في الـ Storefront الأول قبل أي حاجة تانية.
- القرار: كل ما نخلص storefront module، نرجعله بعدين ونضيف الـ Admin endpoints بتاعته (مش هيتعمل كله دفعة واحدة في الآخر).

**3. Bundle → Cart: قرار معلق ولسه محتاج نقاش:**
- زرار "أضيفي إلى السلة" على الـ Bundle cards (في صفحتي Offers و Bundles) **مش شغال حالياً** — بينده `addItem(bundle.id)` اللي هيفشل لأن الـ bundle id مش Product id حقيقي.
- ده مطابق تماماً لسلوك الـ mock الأصلي (كان برضه مش شغال فعلياً) — يعني مفيش أي رجوع للخلف في الوظائف الموجودة.
- **القرار اللي لسه معلق**: هل إضافة Bundle للسلة معناها توسيعه لـ N × CartItem منفصلة (كل منتج بسعره الأصلي، والخصم يتطبق إزاي وقتها؟)، ولا نعمل concept جديد زي `BundleCartItem`؟ محتاج نقاش منفصل قبل أي تنفيذ.

**4. ليه Testimonial مش Review:**
- الـ entity بتاع "آراء العملاء" في الصفحة الرئيسية اتسمى `Testimonial` عن قصد، مش `Review`.
- السبب: تقييمات المنتجات الحالية (`Product.Rating` / `Product.ReviewsCount`) أرقام مجمّعة بس، مفيش جدول Review منفصل لكل منتج لحد دلوقتي. لو حبينا نضيف "مراجعات لكل منتج" (Customer يكتب Review على منتج معين) في المستقبل، اسم `Review` هيبقى متاح ومناسب له بدل ما يتعارض مع الـ testimonials العامة اللي في الهوم بيدج.

### إعدادات البيئة المحلية (Local Dev)
- **Backend**: `http://localhost:5080` (متظبط في `launchSettings.json`، بروفايل "http" الافتراضي)
- **Frontend**: `http://localhost:5173` (Vite default)
- **Database**: SQL Server LocalDB، اسم الداتابيز `LuxiraDb`، الـ connection string متخزن في User Secrets مش appsettings.json
- **CORS**: مسموح بس لـ `http://localhost:5173`
- **Coupon تجريبي متزروع في الـ seed data**: `WELCOME10` (خصم 10%)
- لو حصل port collision (الباك إند مش شغال على 5080 أو الفرونت مش شغال على 5173)، الاحتمال الأكبر إن فيه process قديم من session سابقة لسه شغال على نفس البورت ولازم يتقفل الأول
