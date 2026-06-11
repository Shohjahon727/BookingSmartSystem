using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingAndPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Properties" DROP COLUMN IF EXISTS "BadRooms";
                """);

            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public'
                          AND table_name = 'Properties'
                          AND column_name = 'BathRooms'
                    ) THEN
                        ALTER TABLE "Properties" RENAME COLUMN "BathRooms" TO "Bathrooms";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public'
                          AND table_name = 'Properties'
                          AND column_name = 'Tilte'
                    ) THEN
                        ALTER TABLE "Properties" RENAME COLUMN "Tilte" TO "Title";
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public'
                          AND table_name = 'Properties'
                          AND column_name = 'Bathrooms'
                          AND data_type = 'text'
                    ) THEN
                        ALTER TABLE "Properties"
                        ALTER COLUMN "Bathrooms" TYPE integer
                        USING CASE
                            WHEN "Bathrooms" ~ '^[0-9]+$' THEN "Bathrooms"::integer
                            ELSE 0
                        END;
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql("""
                ALTER TABLE "Properties"
                ADD COLUMN IF NOT EXISTS "Bedrooms" integer NOT NULL DEFAULT 0;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Properties" DROP COLUMN IF EXISTS "Bedrooms";
                """);

            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public'
                          AND table_name = 'Properties'
                          AND column_name = 'Bathrooms'
                          AND data_type = 'integer'
                    ) THEN
                        ALTER TABLE "Properties"
                        ALTER COLUMN "Bathrooms" TYPE text
                        USING "Bathrooms"::text;
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public'
                          AND table_name = 'Properties'
                          AND column_name = 'Bathrooms'
                    ) THEN
                        ALTER TABLE "Properties" RENAME COLUMN "Bathrooms" TO "BathRooms";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public'
                          AND table_name = 'Properties'
                          AND column_name = 'Title'
                    ) THEN
                        ALTER TABLE "Properties" RENAME COLUMN "Title" TO "Tilte";
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql("""
                ALTER TABLE "Properties"
                ADD COLUMN IF NOT EXISTS "BadRooms" text NOT NULL DEFAULT '';
                """);
        }
    }
}
