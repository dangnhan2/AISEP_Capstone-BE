-- =============================================
-- Script ?? chèn d? li?u test cho AISEP
-- T?o 1 User v?i c? Investor Profile và Startup Profile
-- =============================================

-- B?t ??u transaction
BEGIN TRANSACTION;

DECLARE @UserId INT;
DECLARE @InvestorProfileId INT;
DECLARE @StartupProfileId INT;

-- =============================================
-- 1. Chèn User
-- =============================================
-- L?u ý: PasswordHash này là ví d?, b?n nên hash password th?t s?
INSERT INTO users (
    email,
    full_name,
    password_hash,
    role,
    is_active,
    email_verified,
    phone_number,
    profile_image_url,
    created_at,
    updated_at,
    is_deleted,
    deleted_at
)
VALUES (
    'testuser@aisep.com',                                    -- email
    N'Nguy?n V?n Test',                                      -- full_name (N prefix ?? support ti?ng Vi?t)
    '$2a$11$abcdefghijklmnopqrstuvwxyz1234567890ABCDEF',    -- password_hash (example bcrypt hash)
    'Investor',                                              -- role (string, không ph?i int)
    1,                                                       -- is_active
    1,                                                       -- email_verified
    '+84901234567',                                          -- phone_number
    NULL,                                                    -- profile_image_url
    GETUTCDATE(),                                           -- created_at
    NULL,                                                    -- updated_at
    0,                                                       -- is_deleted
    NULL                                                     -- deleted_at
);

-- L?y ID c?a User v?a t?o
SET @UserId = SCOPE_IDENTITY();

PRINT 'User created with ID: ' + CAST(@UserId AS VARCHAR);

-- =============================================
-- 2. Chèn Investor Profile
-- =============================================
INSERT INTO investor_profiles (
    user_id,
    organization_name,
    investment_thesis,
    preferred_industries,
    preferred_stages,
    min_investment_size,
    max_investment_size,
    geographic_focus,
    portfolio_companies,
    website,
    is_published,
    created_at,
    updated_at,
    is_deleted,
    deleted_at
)
VALUES (
    @UserId,                                                 -- user_id
    N'Test Venture Capital',                                -- organization_name
    N'We invest in early-stage technology startups with strong founding teams and innovative solutions in AI, fintech, and healthtech sectors.',  -- investment_thesis
    '["Technology","Fintech","AI/ML","Healthtech"]',        -- preferred_industries (JSON string)
    '["Seed","SeriesA","SeriesB"]',                         -- preferred_stages (JSON string)
    50000.00,                                               -- min_investment_size
    5000000.00,                                             -- max_investment_size
    '["Vietnam","Southeast Asia","United States"]',         -- geographic_focus (JSON string)
    '["Company A","Company B","Company C"]',                -- portfolio_companies (JSON string)
    'https://www.testvc.com',                               -- website
    1,                                                       -- is_published
    GETUTCDATE(),                                           -- created_at
    NULL,                                                    -- updated_at
    0,                                                       -- is_deleted
    NULL                                                     -- deleted_at
);

-- L?y ID c?a Investor Profile v?a t?o
SET @InvestorProfileId = SCOPE_IDENTITY();

PRINT 'Investor Profile created with ID: ' + CAST(@InvestorProfileId AS VARCHAR);

-- =============================================
-- 3. Chèn Startup Profile
-- =============================================
-- L?u ý: Cùng m?t User không th? có c? InvestorProfile VÀ StartupProfile trong logic th?t
-- ?ây ch? là ?? test API. Trong th?c t?, m?t user ch? có m?t lo?i profile.
INSERT INTO startup_profiles (
    user_id,
    company_name,
    logo_url,
    cover_image_url,
    industry,
    stage,
    founding_date,
    team_size,
    location,
    website,
    description,
    funding_amount_sought,
    is_published,
    created_at,
    updated_at,
    is_deleted,
    deleted_at
)
VALUES (
    @UserId,                                                 -- user_id (cùng user)
    N'Test Startup Company',                                -- company_name
    NULL,                                                    -- logo_url
    NULL,                                                    -- cover_image_url
    'Technology',                                            -- industry
    'Seed',                                                  -- stage (string enum)
    '2023-01-15',                                           -- founding_date
    10,                                                      -- team_size
    N'Ho Chi Minh City, Vietnam',                           -- location
    'https://www.teststartup.com',                          -- website
    N'Test Startup is an innovative technology company focused on AI-powered solutions for small and medium enterprises. We help businesses automate their operations and improve efficiency.',  -- description
    500000.00,                                              -- funding_amount_sought
    1,                                                       -- is_published
    GETUTCDATE(),                                           -- created_at
    NULL,                                                    -- updated_at
    0,                                                       -- is_deleted
    NULL                                                     -- deleted_at
);

-- L?y ID c?a Startup Profile v?a t?o
SET @StartupProfileId = SCOPE_IDENTITY();

PRINT 'Startup Profile created with ID: ' + CAST(@StartupProfileId AS VARCHAR);

-- =============================================
-- Commit transaction
-- =============================================
COMMIT TRANSACTION;

-- =============================================
-- 4. SELECT k?t qu? ?? dùng cho test API
-- =============================================
PRINT '============================================='
PRINT 'Test Data Created Successfully!'
PRINT '============================================='

SELECT 
    u.id AS UserId,
    u.email AS UserEmail,
    u.full_name AS UserFullName,
    u.role AS UserRole,
    ip.id AS InvestorProfileId,
    ip.organization_name AS InvestorOrgName,
    sp.id AS StartupProfileId,
    sp.company_name AS StartupCompanyName
FROM users u
LEFT JOIN investor_profiles ip ON ip.user_id = u.id
LEFT JOIN startup_profiles sp ON sp.user_id = u.id
WHERE u.id = @UserId;

PRINT ''
PRINT 'IDs for testing API:'
PRINT '--------------------------------------------'
PRINT 'User ID: ' + CAST(@UserId AS VARCHAR)
PRINT 'Investor Profile ID: ' + CAST(@InvestorProfileId AS VARCHAR)
PRINT 'Startup Profile ID: ' + CAST(@StartupProfileId AS VARCHAR)
PRINT '============================================='

-- =============================================
-- Cleanup script (n?u c?n xóa d? li?u test)
-- =============================================
/*
-- Uncomment ?? xóa d? li?u test
DELETE FROM startup_profiles WHERE user_id = @UserId;
DELETE FROM investor_profiles WHERE user_id = @UserId;
DELETE FROM users WHERE id = @UserId;
*/
