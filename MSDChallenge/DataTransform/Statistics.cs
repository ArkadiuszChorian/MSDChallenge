using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace DataTransform
{
    public class Statistics
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public int SeparatedSchemaUniqueTracksInMemoryElapsedSeconds { get; set; } = 0;
        public int SeparatedSchemaUniqueTracksToDbElapsedSeconds { get; set; } = 0;
        public int SeparatedSchemaListensInMemoryElapsedSeconds { get; set; } = 0;
        public int SeparatedSchemaListensToDbElapsedSeconds { get; set; } = 0;
        public int SeparatedSchemaTotalElapsedSeconds { get; set; } = 0;

        public int PlainSchemaInMemoryElapsedSeconds { get; set; } = 0;
        public int PlainSchemaToDbElapsedSeconds { get; set; } = 0;
        public int PlainSchemaTotalElapsedSeconds { get; set; } = 0;
    }
}
