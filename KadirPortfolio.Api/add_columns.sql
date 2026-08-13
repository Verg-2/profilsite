ALTER TABLE "BlogPosts" ADD COLUMN IF NOT EXISTS "PostType" text NOT NULL DEFAULT 'article';
ALTER TABLE "BlogPosts" ADD COLUMN IF NOT EXISTS "BookColor" text;
SELECT column_name FROM information_schema.columns WHERE table_name = 'BlogPosts';
