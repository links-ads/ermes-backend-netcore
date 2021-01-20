using Ermes.EntityFrameworkCore;

namespace Ermes.Tests.TestDatas
{
    public class TestDataBuilder
    {
        private readonly ErmesDbContext _context;

        public TestDataBuilder(ErmesDbContext context)
        {
            _context = context;
        }

        public void Build()
        {
            //create test data here...
        }
    }
}