-- =============================================
-- Script ?? chèn d? li?u test ?ÚNG THEO BUSINESS LOGIC
-- T?o 2 Users: 1 Investor và 1 Startup Founder
-- =============================================

-- B?t ??u transaction
BEGIN TRANSACTION;

DECLARE @InvestorUserId INT;
DECLARE @StartupUserId INT;
DECLARE @InvestorProfileId INT;
DECLARE @StartupProfileId INT;

-- =============================================
-- 1. Chèn User - Investor
-- =============================================
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
    'investor@aisep.com',                                    -- email
    N'Tr?n Minh Investor',                                   -- full_name
    '$2a$11$InvestorPasswordHash123456789012345678901234',  -- password_hash (example)
    'Investor',                                              -- role
    1,                                                       -- is_active
    1,                                                       -- email_verified
    '+84909123456',                                          -- phone_number
    NULL,                                                    -- profile_image_url
    GETUTCDATE(),                                           -- created_at
    NULL,                                                    -- updated_at
    0,                                                       -- is_deleted
    NULL                                                     -- deleted_at
);

SET @InvestorUserId = SCOPE_IDENTITY();
PRINT 'Investor User created with ID: ' + CAST(@InvestorUserId AS VARCHAR);

-- =============================================
-- 2. Chèn Investor Profile cho Investor User
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
    @InvestorUserId,                                         -- user_id
    N'VietVentures Capital',                                 -- organization_name
    N'VietVentures focuses on investing in high-growth technology startups in Southeast Asia. We partner with entrepreneurs who are building innovative products that solve real problems and have the potential to scale regionally.',  -- investment_thesis
    '["Technology","E-commerce","Fintech","AI/ML","SaaS","EdTech"]',  -- preferred_industries
    '["PreSeed","Seed","SeriesA"]',                         -- preferred_stages
    100000.00,                                              -- min_investment_size
    2000000.00,                                             -- max_investment_size
    '["Vietnam","Thailand","Singapore","Indonesia"]',       -- geographic_focus
    '["TechCorp Vietnam","FinPay","EduLearn","HealthTech Plus"]',  -- portfolio_companies
    'https://www.vietventures.vc',                          -- website
    1,                                                       -- is_published
    GETUTCDATE(),                                           -- created_at
    NULL,                                                    -- updated_at
    0,                                                       -- is_deleted
    NULL                                                     -- deleted_at
);

SET @InvestorProfileId = SCOPE_IDENTITY();
PRINT 'Investor Profile created with ID: ' + CAST(@InvestorProfileId AS VARCHAR);

-- =============================================
-- 3. Chèn User - Startup Founder
-- =============================================
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
    'founder@techstartup.com',                              -- email
    N'Lê Th? Startup',                                       -- full_name
    '$2a$11$FounderPasswordHash12345678901234567890123',   -- password_hash (example)
    'StartupFounder',                                        -- role
    1,                                                       -- is_active
    1,                                                       -- email_verified
    '+84918234567',                                          -- phone_number
    NULL,                                                    -- profile_image_url
    GETUTCDATE(),                                           -- created_at
    NULL,                                                    -- updated_at
    0,                                                       -- is_deleted
    NULL                                                     -- deleted_at
);

SET @StartupUserId = SCOPE_IDENTITY();
PRINT 'Startup Founder User created with ID: ' + CAST(@StartupUserId AS VARCHAR);

-- =============================================
-- 4. Chèn Startup Profile cho Startup Founder User
-- =============================================
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
    @StartupUserId,                                          -- user_id
    N'AI Solutions Vietnam',                                 -- company_name
    NULL,                                                    -- logo_url
    NULL,                                                    -- cover_image_url
    'AI/ML',                                                 -- industry
    'Seed',                                                  -- stage
    '2023-06-01',                                           -- founding_date
    8,                                                       -- team_size
    N'Ho Chi Minh City, Vietnam',                           -- location
    'https://www.aisolutions.vn',                           -- website
    N'AI Solutions Vietnam develops cutting-edge AI-powered analytics tools for small and medium businesses. Our platform helps companies make data-driven decisions by providing real-time insights, predictive analytics, and automated reporting. We are currently serving 50+ customers and growing rapidly.',  -- description
    750000.00,                                              -- funding_amount_sought
    1,                                                       -- is_published
    GETUTCDATE(),                                           -- created_at
    NULL,                                                    -- updated_at
    0,                                                       -- is_deleted
    NULL                                                     -- deleted_at
);

SET @StartupProfileId = SCOPE_IDENTITY();
PRINT 'Startup Profile created with ID: ' + CAST(@StartupProfileId AS VARCHAR);

-- =============================================
-- Commit transaction
-- =============================================
COMMIT TRANSACTION;

-- =============================================
-- 5. SELECT k?t qu? ?? dùng cho test API
-- =============================================
PRINT ''
PRINT '============================================='
PRINT 'Test Data Created Successfully!'
PRINT '============================================='

-- Investor Information
SELECT 
    u.id AS UserId,
    u.email AS UserEmail,
    u.full_name AS UserFullName,
    u.role AS UserRole,
    ip.id AS InvestorProfileId,
    ip.organization_name AS OrganizationName,
    ip.min_investment_size AS MinInvestment,
    ip.max_investment_size AS MaxInvestment,
    ip.is_published AS IsPublished
FROM users u
INNER JOIN investor_profiles ip ON ip.user_id = u.id
WHERE u.id = @InvestorUserId;

-- Startup Information
SELECT 
    u.id AS UserId,
    u.email AS UserEmail,
    u.full_name AS UserFullName,
    u.role AS UserRole,
    sp.id AS StartupProfileId,
    sp.company_name AS CompanyName,
    sp.industry AS Industry,
    sp.stage AS Stage,
    sp.funding_amount_sought AS FundingSought,
    sp.is_published AS IsPublished
FROM users u
INNER JOIN startup_profiles sp ON sp.user_id = u.id
WHERE u.id = @StartupUserId;

PRINT ''
PRINT 'IDs for testing API:'
PRINT '--------------------------------------------'
PRINT 'Investor User ID: ' + CAST(@InvestorUserId AS VARCHAR)
PRINT 'Investor Profile ID: ' + CAST(@InvestorProfileId AS VARCHAR)
PRINT ''
PRINT 'Startup User ID: ' + CAST(@StartupUserId AS VARCHAR)
PRINT 'Startup Profile ID: ' + CAST(@StartupProfileId AS VARCHAR)
PRINT '============================================='

-- =============================================
-- Cleanup script (n?u c?n xóa d? li?u test)
-- =============================================
/*
-- Uncomment ?? xóa d? li?u test
DELETE FROM investor_profiles WHERE user_id = @InvestorUserId;
DELETE FROM users WHERE id = @InvestorUserId;

DELETE FROM startup_profiles WHERE user_id = @StartupUserId;
DELETE FROM users WHERE id = @StartupUserId;
*/
