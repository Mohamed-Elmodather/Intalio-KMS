using AFC27.KMS.Identity.Domain.Entities;
using AFC27.KMS.Content.Domain.Entities;
using AFC27.KMS.Documents.Domain.Entities;
using AFC27.KMS.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;

namespace AFC27.KMS.WebApi.Data;

/// <summary>
/// Seeds the database with AFC 2027 themed demo data.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(KmsDbContext context)
    {
        // Check if already seeded
        if (await context.Users.AnyAsync())
        {
            return;
        }

        // Seed departments
        var departments = SeedDepartments();
        await context.Departments.AddRangeAsync(departments);
        await context.SaveChangesAsync();

        // Seed roles
        var roles = SeedRoles();
        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();

        // Seed users (with departments)
        var users = SeedUsers(departments);
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // Seed categories
        var categories = SeedCategories();
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        // Seed tags
        var tags = SeedTags();
        await context.Tags.AddRangeAsync(tags);
        await context.SaveChangesAsync();

        // Seed articles
        var articles = SeedArticles(users, categories);
        await context.Articles.AddRangeAsync(articles);
        await context.SaveChangesAsync();

        // Seed document libraries
        var libraries = SeedDocumentLibraries(users);
        await context.DocumentLibraries.AddRangeAsync(libraries);
        await context.SaveChangesAsync();

        // Seed folders
        var folders = SeedFolders(libraries);
        await context.Folders.AddRangeAsync(folders);
        await context.SaveChangesAsync();

        // Seed documents
        var documents = SeedDocuments(libraries, folders, users);
        await context.Documents.AddRangeAsync(documents);
        await context.SaveChangesAsync();
    }

    private static List<Department> SeedDepartments()
    {
        return new List<Department>
        {
            CreateDepartment("Operations", "العمليات", "Tournament Operations and Logistics", 1),
            CreateDepartment("Media & Communications", "الإعلام والاتصالات", "Media Relations and Communications", 2),
            CreateDepartment("Venues", "المنشآت", "Stadium and Venue Management", 3),
            CreateDepartment("Ticketing", "التذاكر", "Ticketing and Access Management", 4),
            CreateDepartment("Security", "الأمن", "Security and Safety Operations", 5),
            CreateDepartment("Information Technology", "تقنية المعلومات", "IT and Digital Services", 6),
            CreateDepartment("Human Resources", "الموارد البشرية", "HR and Workforce Management", 7),
            CreateDepartment("Marketing", "التسويق", "Marketing and Brand Management", 8),
            CreateDepartment("Volunteers", "المتطوعين", "Volunteer Program Management", 9),
            CreateDepartment("Legal", "الشؤون القانونية", "Legal and Compliance", 10)
        };
    }

    private static Department CreateDepartment(string name, string nameAr, string description, int sortOrder)
    {
        var dept = Department.Create(name, nameAr, description);
        // Use reflection to set SortOrder since it's a private setter
        typeof(Department).GetProperty(nameof(Department.SortOrder))!
            .SetValue(dept, sortOrder);
        return dept;
    }

    private static List<Role> SeedRoles()
    {
        return new List<Role>
        {
            Role.Create("Administrator", "مدير النظام", "Full system access", "صلاحيات كاملة للنظام", true),
            Role.Create("Department Manager", "مدير قسم", "Department level management", "إدارة مستوى القسم", true),
            Role.Create("Content Editor", "محرر المحتوى", "Create and edit content", "إنشاء وتحرير المحتوى", true),
            Role.Create("Content Reviewer", "مراجع المحتوى", "Review and approve content", "مراجعة واعتماد المحتوى", true),
            Role.Create("Viewer", "مشاهد", "View-only access", "صلاحية العرض فقط", true)
        };
    }

    private static List<User> SeedUsers(List<Department> departments)
    {
        var users = new List<User>();
        var deptIndex = 0;

        // AFC 2027 Staff with Arabic names
        var staffData = new[]
        {
            ("Ahmed Al-Harbi", "أحمد الحربي", "ahmed.alharbi@afc2027.com", "Operations Director", "مدير العمليات"),
            ("Sara Al-Mutairi", "سارة المطيري", "sara.almutairi@afc2027.com", "Media Director", "مديرة الإعلام"),
            ("Mohammed Al-Dosari", "محمد الدوسري", "mohammed.aldosari@afc2027.com", "Venues Director", "مدير المنشآت"),
            ("Fatima Al-Zahrani", "فاطمة الزهراني", "fatima.alzahrani@afc2027.com", "Ticketing Manager", "مديرة التذاكر"),
            ("Khalid Al-Shammari", "خالد الشمري", "khalid.alshammari@afc2027.com", "Security Director", "مدير الأمن"),
            ("Noura Al-Qahtani", "نورة القحطاني", "noura.alqahtani@afc2027.com", "IT Director", "مديرة تقنية المعلومات"),
            ("Abdullah Al-Ghamdi", "عبدالله الغامدي", "abdullah.alghamdi@afc2027.com", "HR Director", "مدير الموارد البشرية"),
            ("Maha Al-Otaibi", "مها العتيبي", "maha.alotaibi@afc2027.com", "Marketing Director", "مديرة التسويق"),
            ("Faisal Al-Rashid", "فيصل الراشد", "faisal.alrashid@afc2027.com", "Volunteer Coordinator", "منسق المتطوعين"),
            ("Layla Al-Harthy", "ليلى الحارثي", "layla.alharthy@afc2027.com", "Legal Counsel", "المستشارة القانونية"),
            ("Omar Al-Subaie", "عمر السبيعي", "omar.alsubaie@afc2027.com", "Content Editor", "محرر المحتوى"),
            ("Reem Al-Juhani", "ريم الجهني", "reem.aljuhani@afc2027.com", "Communications Specialist", "أخصائية الاتصالات"),
            ("Yousef Al-Enezi", "يوسف العنزي", "yousef.alenezi@afc2027.com", "Technical Lead", "القائد التقني"),
            ("Haya Al-Tamimi", "هيا التميمي", "haya.altamimi@afc2027.com", "Event Coordinator", "منسقة الفعاليات"),
            ("Saud Al-Malki", "سعود المالكي", "saud.almalki@afc2027.com", "System Administrator", "مدير النظام")
        };

        foreach (var (name, nameAr, email, title, titleAr) in staffData)
        {
            var user = User.Create(email, name, nameAr);
            SetUserProperties(user, title, titleAr, departments[deptIndex % departments.Count].Id);
            users.Add(user);
            deptIndex++;
        }

        return users;
    }

    private static void SetUserProperties(User user, string jobTitle, string jobTitleAr, Guid departmentId)
    {
        // Use reflection to set properties with private setters
        var type = typeof(User);
        type.GetProperty(nameof(User.JobTitle))!.SetValue(user, jobTitle);
        type.GetProperty(nameof(User.JobTitleArabic))!.SetValue(user, jobTitleAr);
        type.GetProperty(nameof(User.DepartmentId))!.SetValue(user, departmentId);
    }

    private static List<Category> SeedCategories()
    {
        return new List<Category>
        {
            Category.Create(LocalizedString.Create("News", "الأخبار"), LocalizedString.Create("Latest tournament news", "آخر أخبار البطولة")),
            Category.Create(LocalizedString.Create("Venues", "الملاعب"), LocalizedString.Create("Stadium and venue information", "معلومات الملاعب والمنشآت")),
            Category.Create(LocalizedString.Create("Teams", "المنتخبات"), LocalizedString.Create("Participating teams information", "معلومات المنتخبات المشاركة")),
            Category.Create(LocalizedString.Create("Ticketing", "التذاكر"), LocalizedString.Create("Ticketing information and updates", "معلومات وتحديثات التذاكر")),
            Category.Create(LocalizedString.Create("Volunteers", "المتطوعين"), LocalizedString.Create("Volunteer program updates", "تحديثات برنامج التطوع")),
            Category.Create(LocalizedString.Create("Operations", "العمليات"), LocalizedString.Create("Operational updates", "التحديثات التشغيلية")),
            Category.Create(LocalizedString.Create("Media", "الإعلام"), LocalizedString.Create("Media and press information", "معلومات إعلامية وصحفية")),
            Category.Create(LocalizedString.Create("Legacy", "الإرث"), LocalizedString.Create("Tournament legacy and impact", "إرث وأثر البطولة"))
        };
    }

    private static List<Tag> SeedTags()
    {
        return new List<Tag>
        {
            Tag.Create(LocalizedString.Create("AFC2027", "آسيا2027"), "#1E40AF"),
            Tag.Create(LocalizedString.Create("Football", "كرة القدم"), "#059669"),
            Tag.Create(LocalizedString.Create("Saudi Arabia", "السعودية"), "#047857"),
            Tag.Create(LocalizedString.Create("Venues", "الملاعب"), "#7C3AED"),
            Tag.Create(LocalizedString.Create("Ticketing", "التذاكر"), "#DB2777"),
            Tag.Create(LocalizedString.Create("Volunteers", "المتطوعين"), "#EA580C"),
            Tag.Create(LocalizedString.Create("Teams", "المنتخبات"), "#0891B2"),
            Tag.Create(LocalizedString.Create("Legacy", "الإرث"), "#CA8A04"),
            Tag.Create(LocalizedString.Create("Operations", "العمليات"), "#4F46E5"),
            Tag.Create(LocalizedString.Create("Media", "الإعلام"), "#DC2626")
        };
    }

    private static List<Article> SeedArticles(List<User> users, List<Category> categories)
    {
        var articles = new List<Article>();
        var author = users.First();
        var newsCategory = categories.First();

        var articlesData = new[]
        {
            (
                "AFC Asian Cup 2027 Venues Officially Announced",
                "الإعلان الرسمي عن ملاعب كأس آسيا 2027",
                "The Saudi Arabian Football Federation has officially unveiled the venues for AFC Asian Cup 2027. Eight world-class stadiums across four host cities will welcome teams and fans from across Asia.",
                "أعلن الاتحاد السعودي لكرة القدم رسمياً عن ملاعب كأس آسيا 2027. ستستقبل ثمانية ملاعب عالمية المستوى في أربع مدن مضيفة الفرق والمشجعين من جميع أنحاء آسيا.",
                ArticleType.News
            ),
            (
                "Volunteer Registration Opens for AFC Asian Cup 2027",
                "فتح باب التسجيل للمتطوعين في كأس آسيا 2027",
                "Be part of history! The volunteer program for AFC Asian Cup 2027 is now accepting applications. Join thousands of passionate individuals in delivering an unforgettable tournament experience.",
                "كن جزءاً من التاريخ! برنامج التطوع لكأس آسيا 2027 يستقبل الآن الطلبات. انضم إلى آلاف المتحمسين في تقديم تجربة بطولة لا تُنسى.",
                ArticleType.News
            ),
            (
                "Qualification Path Confirmed for AFC Asian Cup 2027",
                "تأكيد مسار التأهل لكأس آسيا 2027",
                "The Asian Football Confederation has confirmed the qualification pathway for the 2027 tournament. 24 teams will compete in the expanded format, providing more opportunities for Asian nations.",
                "أكد الاتحاد الآسيوي لكرة القدم مسار التأهل لبطولة 2027. ستتنافس 24 فريقاً في الشكل الموسع، مما يوفر المزيد من الفرص للدول الآسيوية.",
                ArticleType.News
            ),
            (
                "Ticketing Information for AFC Asian Cup 2027",
                "معلومات التذاكر لكأس آسيا 2027",
                "Planning to attend AFC Asian Cup 2027? Here's everything you need to know about ticket sales, pricing categories, and how to secure your seats for the biggest football event in Asia.",
                "تخطط لحضور كأس آسيا 2027؟ إليك كل ما تحتاج معرفته عن مبيعات التذاكر وفئات الأسعار وكيفية حجز مقاعدك لأكبر حدث كروي في آسيا.",
                ArticleType.Article
            ),
            (
                "State-of-the-Art Technology at AFC Asian Cup 2027",
                "تقنيات متطورة في كأس آسيا 2027",
                "AFC Asian Cup 2027 will showcase cutting-edge technology including VAR, goal-line technology, and advanced stadium connectivity. Experience football like never before.",
                "ستعرض كأس آسيا 2027 أحدث التقنيات بما في ذلك VAR وتقنية خط المرمى والاتصال المتقدم في الملاعب. عش تجربة كرة القدم كما لم تعشها من قبل.",
                ArticleType.Article
            ),
            (
                "Sustainability Goals for AFC Asian Cup 2027",
                "أهداف الاستدامة لكأس آسيا 2027",
                "The organizing committee is committed to hosting a sustainable tournament. Learn about the environmental initiatives and carbon neutrality goals for AFC Asian Cup 2027.",
                "تلتزم اللجنة المنظمة باستضافة بطولة مستدامة. تعرف على المبادرات البيئية وأهداف الحياد الكربوني لكأس آسيا 2027.",
                ArticleType.Article
            ),
            (
                "Fan Zones Announced Across Saudi Arabia",
                "الإعلان عن مناطق المشجعين في أنحاء السعودية",
                "Can't make it to the stadium? Fan zones will be set up in major cities across Saudi Arabia, offering live screenings, entertainment, and the ultimate matchday atmosphere.",
                "لا تستطيع الوصول للملعب؟ سيتم إنشاء مناطق للمشجعين في المدن الرئيسية في السعودية، تقدم بث مباشر وترفيه وأجواء يوم المباراة المثالية.",
                ArticleType.News
            ),
            (
                "Transportation Plan for AFC Asian Cup 2027",
                "خطة النقل لكأس آسيا 2027",
                "Detailed transportation plans including dedicated bus routes, metro services, and parking facilities have been announced to ensure smooth movement of fans during the tournament.",
                "تم الإعلان عن خطط نقل تفصيلية تشمل خطوط حافلات مخصصة وخدمات مترو ومرافق مواقف لضمان حركة سلسة للمشجعين خلال البطولة.",
                ArticleType.Article
            ),
            (
                "Media Accreditation Opens for AFC Asian Cup 2027",
                "فتح باب الاعتماد الإعلامي لكأس آسيا 2027",
                "Journalists and media professionals can now apply for accreditation. The media program includes world-class facilities, interview zones, and comprehensive broadcast coverage.",
                "يمكن للصحفيين والإعلاميين التقدم الآن للاعتماد. يشمل البرنامج الإعلامي مرافق عالمية المستوى ومناطق مقابلات وتغطية بث شاملة.",
                ArticleType.News
            ),
            (
                "Legacy Projects Announced for Host Cities",
                "الإعلان عن مشاريع الإرث للمدن المضيفة",
                "AFC Asian Cup 2027 will leave a lasting legacy. New sports facilities, community programs, and infrastructure improvements will benefit local communities for years to come.",
                "ستترك كأس آسيا 2027 إرثاً دائماً. مرافق رياضية جديدة وبرامج مجتمعية وتحسينات في البنية التحتية ستفيد المجتمعات المحلية لسنوات قادمة.",
                ArticleType.Article
            ),
            (
                "Opening Ceremony Preview: A Celebration of Asian Football",
                "نظرة على حفل الافتتاح: احتفال بكرة القدم الآسيوية",
                "The opening ceremony promises to be a spectacular showcase of Asian culture, heritage, and the spirit of football. Get a sneak peek at what to expect.",
                "يعد حفل الافتتاح بأن يكون عرضاً مذهلاً للثقافة والتراث الآسيوي وروح كرة القدم. ألقِ نظرة خاطفة على ما يمكن توقعه.",
                ArticleType.News
            ),
            (
                "Accommodation Guide for AFC Asian Cup 2027",
                "دليل الإقامة لكأس آسيا 2027",
                "From luxury hotels to budget-friendly options, find the perfect place to stay during AFC Asian Cup 2027. Our comprehensive guide covers all host cities.",
                "من الفنادق الفاخرة إلى الخيارات الاقتصادية، اعثر على المكان المثالي للإقامة خلال كأس آسيا 2027. دليلنا الشامل يغطي جميع المدن المضيفة.",
                ArticleType.Article
            ),
            (
                "Youth Football Programs Launched Ahead of Tournament",
                "إطلاق برامج كرة القدم للشباب قبل البطولة",
                "Inspiring the next generation: Youth development programs and school initiatives are being rolled out across Saudi Arabia as part of the AFC Asian Cup 2027 preparations.",
                "إلهام الجيل القادم: يتم إطلاق برامج تطوير الشباب والمبادرات المدرسية في جميع أنحاء السعودية كجزء من استعدادات كأس آسيا 2027.",
                ArticleType.News
            ),
            (
                "Match Schedule Released for Group Stage",
                "إصدار جدول مباريات دور المجموعات",
                "The complete match schedule for the group stage has been released. Plan your tournament experience with our interactive fixture guide.",
                "تم إصدار جدول المباريات الكامل لدور المجموعات. خطط لتجربة البطولة مع دليل المباريات التفاعلي.",
                ArticleType.News
            ),
            (
                "Safety and Security Measures for AFC Asian Cup 2027",
                "إجراءات السلامة والأمن لكأس آسيا 2027",
                "The safety of fans, players, and officials is our top priority. Learn about the comprehensive security measures in place for AFC Asian Cup 2027.",
                "سلامة المشجعين واللاعبين والمسؤولين هي أولويتنا القصوى. تعرف على الإجراءات الأمنية الشاملة المعمول بها لكأس آسيا 2027.",
                ArticleType.Article
            ),
            (
                "Cultural Events Planned During Tournament Period",
                "فعاليات ثقافية مخطط لها خلال فترة البطولة",
                "Beyond football, experience the rich culture of Saudi Arabia. Cultural festivals, exhibitions, and performances will run throughout the tournament.",
                "إلى جانب كرة القدم، عش ثقافة السعودية الغنية. ستقام مهرجانات ومعارض وعروض ثقافية طوال فترة البطولة.",
                ArticleType.Article
            ),
            (
                "Official Mascot Unveiled for AFC Asian Cup 2027",
                "الكشف عن التميمة الرسمية لكأس آسيا 2027",
                "Meet the official mascot of AFC Asian Cup 2027! The character embodies the spirit of Saudi hospitality and Asian football unity.",
                "تعرف على التميمة الرسمية لكأس آسيا 2027! تجسد الشخصية روح الضيافة السعودية ووحدة كرة القدم الآسيوية.",
                ArticleType.News
            ),
            (
                "Broadcast Partners Announced for Global Coverage",
                "الإعلان عن شركاء البث للتغطية العالمية",
                "AFC Asian Cup 2027 will be broadcast to billions of viewers worldwide. Major broadcast agreements ensure comprehensive coverage across all continents.",
                "ستُبث كأس آسيا 2027 لمليارات المشاهدين حول العالم. تضمن اتفاقيات البث الرئيسية تغطية شاملة في جميع القارات.",
                ArticleType.News
            ),
            (
                "Food and Beverage Experience at AFC Asian Cup 2027",
                "تجربة الطعام والمشروبات في كأس آسيا 2027",
                "From local Saudi cuisine to international favorites, the culinary experience at AFC Asian Cup 2027 stadiums will cater to all tastes.",
                "من المأكولات السعودية المحلية إلى الأطباق العالمية المفضلة، ستلبي تجربة الطهي في ملاعب كأس آسيا 2027 جميع الأذواق.",
                ArticleType.Article
            ),
            (
                "Accessibility Services for Fans with Disabilities",
                "خدمات إمكانية الوصول للمشجعين ذوي الإعاقة",
                "AFC Asian Cup 2027 is committed to being accessible to all. Comprehensive services and facilities ensure every fan can enjoy the tournament.",
                "تلتزم كأس آسيا 2027 بأن تكون متاحة للجميع. الخدمات والمرافق الشاملة تضمن استمتاع كل مشجع بالبطولة.",
                ArticleType.Article
            )
        };

        foreach (var (titleEn, titleAr, contentEn, contentAr, type) in articlesData)
        {
            var article = Article.Create(
                LocalizedString.Create(titleEn, titleAr),
                LocalizedString.Create(contentEn, contentAr),
                type,
                author.Id,
                author.DisplayName,
                newsCategory.Id
            );

            // Set as published
            SetArticleAsPublished(article);
            articles.Add(article);
        }

        return articles;
    }

    private static void SetArticleAsPublished(Article article)
    {
        var type = typeof(Article);
        type.GetProperty(nameof(Article.Status))!.SetValue(article, ArticleStatus.Published);
        type.GetProperty(nameof(Article.PublishedAt))!.SetValue(article, DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30)));
    }

    private static List<DocumentLibrary> SeedDocumentLibraries(List<User> users)
    {
        var owner = users.First();
        var libraries = new List<DocumentLibrary>();

        var libraryData = new[]
        {
            ("Tournament Operations", "عمليات البطولة", "Official tournament operations documents", "وثائق عمليات البطولة الرسمية", LibraryType.Project, "#1E40AF"),
            ("Venue Management", "إدارة المنشآت", "Stadium and venue documentation", "وثائق الملاعب والمنشآت", LibraryType.Project, "#059669"),
            ("Media & Communications", "الإعلام والاتصالات", "Press releases and media materials", "البيانات الصحفية والمواد الإعلامية", LibraryType.Department, "#7C3AED"),
            ("Ticketing & Hospitality", "التذاكر والضيافة", "Ticketing policies and hospitality guides", "سياسات التذاكر وأدلة الضيافة", LibraryType.Department, "#DB2777"),
            ("Security & Safety", "الأمن والسلامة", "Security protocols and safety procedures", "بروتوكولات الأمن وإجراءات السلامة", LibraryType.Department, "#DC2626"),
            ("Volunteer Program", "برنامج التطوع", "Volunteer training materials and guides", "مواد تدريب المتطوعين والأدلة", LibraryType.Project, "#EA580C"),
            ("IT & Technology", "تقنية المعلومات", "Technical documentation and specifications", "الوثائق والمواصفات التقنية", LibraryType.Department, "#0891B2"),
            ("Legal & Compliance", "القانونية والامتثال", "Contracts, agreements, and compliance docs", "العقود والاتفاقيات ووثائق الامتثال", LibraryType.Department, "#4F46E5"),
            ("Marketing & Brand", "التسويق والعلامة التجارية", "Brand guidelines and marketing materials", "إرشادات العلامة التجارية والمواد التسويقية", LibraryType.Department, "#F59E0B"),
            ("Training & Development", "التدريب والتطوير", "Training programs and development resources", "برامج التدريب وموارد التطوير", LibraryType.General, "#10B981")
        };

        foreach (var (nameEn, nameAr, descEn, descAr, type, color) in libraryData)
        {
            var library = DocumentLibrary.Create(
                LocalizedString.Create(nameEn, nameAr),
                type,
                owner.Id,
                owner.DisplayName,
                isPublic: true  // All libraries public for demo
            );

            // Set description and icon
            var libType = typeof(DocumentLibrary);
            libType.GetProperty(nameof(DocumentLibrary.Description))!.SetValue(library, LocalizedString.Create(descEn, descAr));
            libType.GetProperty(nameof(DocumentLibrary.Color))!.SetValue(library, color);
            libType.GetProperty(nameof(DocumentLibrary.IconName))!.SetValue(library, GetIconForLibrary(type));

            libraries.Add(library);
        }

        return libraries;
    }

    private static string GetIconForLibrary(LibraryType type) => type switch
    {
        LibraryType.Project => "folder-kanban",
        LibraryType.Department => "building-2",
        LibraryType.Personal => "user",
        LibraryType.Archive => "archive",
        _ => "folder"
    };

    private static List<Folder> SeedFolders(List<DocumentLibrary> libraries)
    {
        var folders = new List<Folder>();

        // Tournament Operations folders
        var opsLibrary = libraries[0];
        var opsFolders = new[]
        {
            ("Match Day Operations", "عمليات يوم المباراة"),
            ("Event Planning", "تخطيط الفعاليات"),
            ("Logistics", "الخدمات اللوجستية"),
            ("Ceremonies", "الاحتفالات"),
            ("Protocols", "البروتوكولات")
        };
        folders.AddRange(CreateFolders(opsFolders, opsLibrary.Id));

        // Venue Management folders
        var venueLibrary = libraries[1];
        var venueFolders = new[]
        {
            ("Stadium Specifications", "مواصفات الملاعب"),
            ("Facility Management", "إدارة المرافق"),
            ("Field Operations", "عمليات الملعب"),
            ("Maintenance Schedules", "جداول الصيانة"),
            ("Capacity Plans", "خطط السعة")
        };
        folders.AddRange(CreateFolders(venueFolders, venueLibrary.Id));

        // Media folders
        var mediaLibrary = libraries[2];
        var mediaFolders = new[]
        {
            ("Press Releases", "البيانات الصحفية"),
            ("Media Kits", "الحقائب الإعلامية"),
            ("Photo Assets", "الصور"),
            ("Video Assets", "الفيديوهات"),
            ("Social Media", "وسائل التواصل الاجتماعي")
        };
        folders.AddRange(CreateFolders(mediaFolders, mediaLibrary.Id));

        // Ticketing folders
        var ticketingLibrary = libraries[3];
        var ticketingFolders = new[]
        {
            ("Pricing Policies", "سياسات التسعير"),
            ("Seating Charts", "مخططات المقاعد"),
            ("VIP Packages", "باقات كبار الشخصيات"),
            ("Group Sales", "مبيعات المجموعات"),
            ("Hospitality Suites", "أجنحة الضيافة")
        };
        folders.AddRange(CreateFolders(ticketingFolders, ticketingLibrary.Id));

        // Security folders
        var securityLibrary = libraries[4];
        var securityFolders = new[]
        {
            ("Security Protocols", "بروتوكولات الأمن"),
            ("Emergency Procedures", "إجراءات الطوارئ"),
            ("Access Control", "التحكم في الدخول"),
            ("Crowd Management", "إدارة الحشود"),
            ("Incident Reports", "تقارير الحوادث")
        };
        folders.AddRange(CreateFolders(securityFolders, securityLibrary.Id));

        // Volunteer folders
        var volunteerLibrary = libraries[5];
        var volunteerFolders = new[]
        {
            ("Training Materials", "مواد التدريب"),
            ("Role Descriptions", "وصف الأدوار"),
            ("Schedules", "الجداول"),
            ("Uniforms & Equipment", "الزي والمعدات"),
            ("Certificates", "الشهادات")
        };
        folders.AddRange(CreateFolders(volunteerFolders, volunteerLibrary.Id));

        return folders;
    }

    private static List<Folder> CreateFolders((string nameEn, string nameAr)[] folderData, Guid libraryId)
    {
        var folders = new List<Folder>();
        var sortOrder = 1;

        foreach (var (nameEn, nameAr) in folderData)
        {
            var folder = Folder.Create(
                LocalizedString.Create(nameEn, nameAr),
                libraryId
            );
            folder.SetSortOrder(sortOrder++);
            folders.Add(folder);
        }

        return folders;
    }

    private static List<Document> SeedDocuments(List<DocumentLibrary> libraries, List<Folder> folders, List<User> users)
    {
        var documents = new List<Document>();
        var random = new Random(42); // Consistent seed for reproducible data

        var documentTemplates = new[]
        {
            // Tournament Operations Documents
            ("AFC 2027 Tournament Operations Manual", "دليل عمليات بطولة آسيا 2027", "application/pdf", ".pdf", 0, 0),
            ("Match Day Checklist Template", "قالب قائمة التحقق ليوم المباراة", "application/pdf", ".pdf", 0, 0),
            ("Opening Ceremony Run of Show", "سيناريو حفل الافتتاح", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx", 0, 3),
            ("Closing Ceremony Plans", "خطط حفل الختام", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx", 0, 3),
            ("Protocol Guidelines", "إرشادات البروتوكول", "application/pdf", ".pdf", 0, 4),

            // Venue Documents
            ("King Fahd Stadium Specifications", "مواصفات ملعب الملك فهد", "application/pdf", ".pdf", 1, 5),
            ("Al-Awwal Park Technical Details", "التفاصيل الفنية لملعب الأول بارك", "application/pdf", ".pdf", 1, 5),
            ("Venue Capacity Analysis", "تحليل سعة الملاعب", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx", 1, 9),
            ("Facility Maintenance Schedule", "جدول صيانة المرافق", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx", 1, 8),
            ("Field Dimensions Standards", "معايير أبعاد الملعب", "application/pdf", ".pdf", 1, 7),

            // Media Documents
            ("Brand Guidelines 2027", "إرشادات العلامة التجارية 2027", "application/pdf", ".pdf", 2, 11),
            ("Press Release Template", "قالب البيان الصحفي", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx", 2, 10),
            ("Media Accreditation Guide", "دليل الاعتماد الإعلامي", "application/pdf", ".pdf", 2, 11),
            ("Social Media Guidelines", "إرشادات وسائل التواصل الاجتماعي", "application/pdf", ".pdf", 2, 14),
            ("Photography Guidelines", "إرشادات التصوير", "application/pdf", ".pdf", 2, 12),

            // Ticketing Documents
            ("Ticketing Policy 2027", "سياسة التذاكر 2027", "application/pdf", ".pdf", 3, 15),
            ("VIP Package Details", "تفاصيل باقات كبار الشخصيات", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx", 3, 17),
            ("Stadium Seating Layout - Riyadh", "مخطط مقاعد ملعب الرياض", "application/pdf", ".pdf", 3, 16),
            ("Group Booking Form", "نموذج الحجز الجماعي", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx", 3, 18),
            ("Hospitality Suite Menu", "قائمة أجنحة الضيافة", "application/pdf", ".pdf", 3, 19),

            // Security Documents
            ("Security Operations Manual", "دليل العمليات الأمنية", "application/pdf", ".pdf", 4, 20),
            ("Emergency Response Plan", "خطة الاستجابة للطوارئ", "application/pdf", ".pdf", 4, 21),
            ("Access Control Procedures", "إجراءات التحكم في الدخول", "application/pdf", ".pdf", 4, 22),
            ("Crowd Management Strategy", "استراتيجية إدارة الحشود", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx", 4, 23),
            ("Incident Report Template", "قالب تقرير الحوادث", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx", 4, 24),

            // Volunteer Documents
            ("Volunteer Handbook", "دليل المتطوع", "application/pdf", ".pdf", 5, 25),
            ("Training Schedule", "جدول التدريب", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx", 5, 27),
            ("Role Description - Guest Services", "وصف الدور - خدمات الضيوف", "application/pdf", ".pdf", 5, 26),
            ("Role Description - Venue Support", "وصف الدور - دعم المنشأة", "application/pdf", ".pdf", 5, 26),
            ("Uniform Guidelines", "إرشادات الزي الرسمي", "application/pdf", ".pdf", 5, 28),
            ("Certificate of Appreciation Template", "قالب شهادة التقدير", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx", 5, 29),

            // IT Documents
            ("IT Infrastructure Overview", "نظرة عامة على البنية التحتية لتقنية المعلومات", "application/pdf", ".pdf", 6, -1),
            ("Network Architecture Diagram", "مخطط بنية الشبكة", "application/pdf", ".pdf", 6, -1),
            ("System Integration Guide", "دليل تكامل الأنظمة", "application/pdf", ".pdf", 6, -1),
            ("Cybersecurity Protocols", "بروتوكولات الأمن السيبراني", "application/pdf", ".pdf", 6, -1),

            // Legal Documents
            ("Sponsorship Agreement Template", "قالب اتفاقية الرعاية", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx", 7, -1),
            ("Broadcast Rights Agreement", "اتفاقية حقوق البث", "application/pdf", ".pdf", 7, -1),
            ("Vendor Contract Template", "قالب عقد المورد", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx", 7, -1),
            ("Data Protection Policy", "سياسة حماية البيانات", "application/pdf", ".pdf", 7, -1),

            // Marketing Documents
            ("Marketing Strategy 2027", "استراتيجية التسويق 2027", "application/pdf", ".pdf", 8, -1),
            ("Campaign Guidelines", "إرشادات الحملة", "application/pdf", ".pdf", 8, -1),
            ("Partner Activation Guide", "دليل تفعيل الشركاء", "application/pdf", ".pdf", 8, -1),
            ("Merchandise Catalog", "كتالوج المنتجات التذكارية", "application/pdf", ".pdf", 8, -1),

            // Training Documents
            ("Customer Service Excellence", "التميز في خدمة العملاء", "application/pdf", ".pdf", 9, -1),
            ("Cultural Awareness Training", "تدريب الوعي الثقافي", "application/pdf", ".pdf", 9, -1),
            ("First Aid Certification Guide", "دليل شهادة الإسعافات الأولية", "application/pdf", ".pdf", 9, -1),
            ("Language Essentials - Arabic", "أساسيات اللغة - العربية", "application/pdf", ".pdf", 9, -1),
            ("Language Essentials - English", "أساسيات اللغة - الإنجليزية", "application/pdf", ".pdf", 9, -1)
        };

        foreach (var (nameEn, nameAr, contentType, extension, libraryIndex, folderIndex) in documentTemplates)
        {
            var library = libraries[libraryIndex];
            var folderId = folderIndex >= 0 && folderIndex < folders.Count ? folders[folderIndex].Id : (Guid?)null;
            var creator = users[random.Next(users.Count)];

            var document = Document.Create(
                nameEn,
                $"{nameEn.ToLower().Replace(" ", "-").Replace("'", "")}{extension}",
                contentType,
                random.Next(50000, 5000000), // Random file size between 50KB and 5MB
                $"documents/{library.Id}/{Guid.NewGuid():N}{extension}",
                library.Id,
                folderId
            );

            // Set document properties
            SetDocumentProperties(document, nameAr, creator.Id);

            documents.Add(document);
        }

        return documents;
    }

    private static void SetDocumentProperties(Document document, string nameArabic, Guid creatorId)
    {
        var type = typeof(Document);
        type.GetProperty(nameof(Document.NameArabic))!.SetValue(document, nameArabic);
        type.GetProperty(nameof(Document.Status))!.SetValue(document, DocumentStatus.Published);

        // Set audit fields via base class
        var baseType = typeof(AuditableEntity);
        baseType.GetProperty(nameof(AuditableEntity.CreatedAt))!.SetValue(document, DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 60)));
        baseType.GetProperty(nameof(AuditableEntity.CreatedBy))!.SetValue(document, creatorId);
    }
}
