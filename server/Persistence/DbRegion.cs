namespace atlantis.Persistence {
    using System.Collections.Generic;

    public class DbRegion {
        public long Id { get; set; }
        public long GameId { get; set; }
        public long TurnId { get; set; }

        public long Number { get; set; }
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }

        public DbGame Game { get; set; }
        public DbTurn Turn { get; set; }

        public List<DbUnit> Units { get; set; }
        public List<DbStructure> Structures { get; set; }
    }
}