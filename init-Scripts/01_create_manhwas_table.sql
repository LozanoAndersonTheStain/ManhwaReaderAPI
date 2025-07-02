-- Create enum types
CREATE TYPE genre AS ENUM
(
    'Action', 'Adventure', 'Comedy', 'Drama', 'Fantasy',
    'Horror', 'Mystery', 'Romance', 'SciFi', 'SliceOfLife',
    'Sports', 'Supernatural', 'Psychological', 'Martial_Arts'
);

CREATE TYPE status AS ENUM
(
    'Ongoing', 'Completed', 'Hiatus', 'Cancelled'
);

-- Create Manhwas table
CREATE TABLE
IF NOT EXISTS Manhwas
(
    Id UUID PRIMARY KEY,
    Title VARCHAR
(255) NOT NULL,
    Description TEXT,
    Author VARCHAR
(255) NOT NULL,
    CoverImage TEXT,
    Genres genre[] NOT NULL,
    Status status NOT NULL,
    ChapterCount INTEGER NOT NULL DEFAULT 0,
    ReleaseDate TIMESTAMP
WITH TIME ZONE NOT NULL,
    LastUpdate TIMESTAMP
WITH TIME ZONE NOT NULL,
    Rating DECIMAL
(3,2) NOT NULL DEFAULT 0.0,
    ViewCount INTEGER NOT NULL DEFAULT 0,
    IsActive BOOLEAN NOT NULL DEFAULT true,
    CreatedAt TIMESTAMP
WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP
WITH TIME ZONE
);

-- Create indexes
CREATE INDEX idx_manhwas_title ON Manhwas(Title);
CREATE INDEX idx_manhwas_author ON Manhwas(Author);
CREATE INDEX idx_manhwas_rating ON Manhwas(Rating);
CREATE INDEX idx_manhwas_lastupdate ON Manhwas(LastUpdate);

-- Sample data insertion
INSERT INTO Manhwas
    (
    Id, Title, Description, Author, CoverImage,
    Genres, Status, ChapterCount, ReleaseDate, LastUpdate,
    Rating, ViewCount, IsActive, CreatedAt
    )
VALUES
    (
        gen_random_uuid(),
        'Solo Leveling',
        'In a world where hunters must battle deadly monsters to protect humanity, Sung Jin-Woo begins as the weakest of hunters before an unexpected turn of events changes his life forever.',
        'Chu-Gong',
        'https://dashboard.olympusbiblioteca.com/storage/comics/covers/230/Fm6u-ngaYAAZKyf-xl.webp',
        ARRAY
['Action', 'Adventure', 'Fantasy']::genre[],
    'Completed',
    179,
    '2018-07-25T00:00:00Z',
    CURRENT_TIMESTAMP,
    4.9,
    1000000,
    true,
    CURRENT_TIMESTAMP
),
(
    gen_random_uuid
(),
    'Tower of God',
    'The story of the Tower of God centers around a boy called Twenty-Fifth Bam, who has spent most of his life trapped beneath a vast and mysterious Tower.',
    'SIU',
    'https://example.com/tower-of-god-cover.jpg',
    ARRAY['Action', 'Adventure', 'Fantasy', 'Mystery']::genre[],
    'Ongoing',
    550,
    '2010-06-30T00:00:00Z',
    CURRENT_TIMESTAMP,
    4.8,
    950000,
    true,
    CURRENT_TIMESTAMP
);
