using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirAnalysis.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterializedView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Видали View якщо існує
            migrationBuilder.Sql(@"
                IF OBJECT_ID('dbo.vw_RecordDataDaily', 'V') IS NOT NULL
                    DROP VIEW dbo.vw_RecordDataDaily;
            ");

            // Створи Materialized View (тільки SUM і COUNT_BIG)
            migrationBuilder.Sql(@"
                CREATE VIEW dbo.vw_RecordDataDaily
                WITH SCHEMABINDING
                AS
                SELECT 
                    rd.PlaceId,
                    CAST(rd.DateRecord AS DATE) AS [Date],
                    rd.PhenomenId,
                    SUM(rd.Value) AS SumValue,
                    COUNT_BIG(*) AS RecordCount
                FROM dbo.RecordData rd
                WHERE rd.PhenomenId IS NOT NULL
                GROUP BY 
                    rd.PlaceId,
                    CAST(rd.DateRecord AS DATE),
                    rd.PhenomenId;
            ");

            // Створи CLUSTERED INDEX (робить View materialized!)
            migrationBuilder.Sql(@"
                CREATE UNIQUE CLUSTERED INDEX idx_RecordDataDaily_Clustered
                ON dbo.vw_RecordDataDaily (PlaceId, [Date], PhenomenId);
            ");

            // Додаткові індекси для оптимізації
            migrationBuilder.Sql(@"
                CREATE NONCLUSTERED INDEX idx_RecordDataDaily_Date
                ON dbo.vw_RecordDataDaily ([Date]) 
                INCLUDE (SumValue, RecordCount);
            ");

            migrationBuilder.Sql(@"
                CREATE NONCLUSTERED INDEX idx_RecordDataDaily_Place
                ON dbo.vw_RecordDataDaily (PlaceId, [Date])
                INCLUDE (SumValue, RecordCount);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // При rollback видаляємо View
            migrationBuilder.Sql(@"
                IF OBJECT_ID('dbo.vw_RecordDataDaily', 'V') IS NOT NULL
                    DROP VIEW dbo.vw_RecordDataDaily;
            ");
        }
    }
}