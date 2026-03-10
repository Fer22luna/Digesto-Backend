-- Create regulations table in Supabase
CREATE TABLE IF NOT EXISTS regulations (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    special_number TEXT NOT NULL,
    reference TEXT NOT NULL,
    type TEXT NOT NULL DEFAULT 'DECREE',
    state TEXT NOT NULL DEFAULT 'DRAFT',
    legal_status TEXT DEFAULT 'SIN_ESTADO',
    content TEXT,
    keywords TEXT[] DEFAULT ARRAY[]::TEXT[],
    publication_date TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    file_url TEXT,
    pdf_url TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    CONSTRAINT type_check CHECK (type IN ('DECREE', 'RESOLUTION', 'ORDINANCE', 'TRIBUNAL_RESOLUTION', 'BID')),
    CONSTRAINT state_check CHECK (state IN ('DRAFT', 'REVIEW', 'PUBLISHED', 'ARCHIVED')),
    CONSTRAINT legal_status_check CHECK (legal_status IN ('VIGENTE', 'PARCIAL', 'SIN_ESTADO'))
);

-- Create indexes for better performance
CREATE INDEX idx_regulations_state ON regulations(state);
CREATE INDEX idx_regulations_type ON regulations(type);
CREATE INDEX idx_regulations_publication_date ON regulations(publication_date DESC);
CREATE INDEX idx_regulations_created_at ON regulations(created_at DESC);

-- Enable full text search on reference and content
CREATE INDEX idx_regulations_reference_search ON regulations USING GIN(to_tsvector('spanish', reference));
CREATE INDEX idx_regulations_content_search ON regulations USING GIN(to_tsvector('spanish', COALESCE(content, '')));

-- Insert sample data
INSERT INTO regulations (special_number, reference, type, state, legal_status, content, keywords, publication_date) VALUES
(
    '27-DEXC-67',
    'DECRETO EXENCIÓN 27-2026',
    'DECREE',
    'PUBLISHED',
    'VIGENTE',
    'Este es el contenido del decreto ejemplo...',
    ARRAY['2025', 'pasodedos'],
    NOW()
),
(
    'RES-001-2026',
    'RESOLUCIÓN 001',
    'RESOLUTION',
    'DRAFT',
    'SIN_ESTADO',
    'Contenido de la resolución...', 
    ARRAY['resolución', 'administrativa'],
    NOW()
);

-- Row Level Security (optional - for public access to published regulations)
ALTER TABLE regulations ENABLE ROW LEVEL SECURITY;

-- Public can see published regulations
CREATE POLICY "Public can view published regulations" 
    ON regulations FOR SELECT 
    USING (state = 'PUBLISHED');

-- Authenticated users can see all regulations (adjust based on your auth setup)
CREATE POLICY "Authenticated users can view all regulations" 
    ON regulations FOR SELECT 
    USING (auth.role() = 'authenticated'::text);
