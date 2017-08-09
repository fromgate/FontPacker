using System.Collections.Generic;

namespace FontPacker
{
    public class McpePackManifest
    {
        public PackHeader header { get; set; }

        public McpePackManifest(Cfg cfg)
        {
            PackModule module = new PackModule
            {
                description = cfg.Description,
                type = "resources",
                version = cfg.Version[0] + "," + cfg.Version[1] + "," + cfg.Version[2],
                uuid = cfg.UuidModule
            };
            header = new PackHeader
            {
                modules = new List<PackModule> {module},
                pack_id = cfg.UuidPack,
                name = cfg.Name,
                description = cfg.Description,
                packs_version = module.version
            };
        }
    }
    
    public class PackModule
    {
        public string description { get; set; }
        public string version { get; set; }
        public string uuid { get; set; }
        public string type { get; set; }
    }

    public class PackHeader
    {
        public string pack_id { get; set; }
        public string name { get; set; }
        public string packs_version { get; set; }
        public string description { get; set; }
        public List<PackModule> modules { get; set; }
    }
}