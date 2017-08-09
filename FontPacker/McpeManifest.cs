using System.Collections.Generic;

namespace FontPacker
{
    public class McpeManifest
    {
        public int format_version { get; set; }
        public Header header { get; set; }
        public List<Module> modules { get; set; }

        public McpeManifest(Cfg cfg)
        {
            format_version = 1;
            header = new Header()
            {
                name = cfg.Name,
                description = cfg.Description,
                uuid = cfg.UuidPack,
                version =  cfg.Version,
            };

            modules = new List<Module>()
            {
                new Module()
                {
                    description = cfg.Description,
                    type = "resources",
                    uuid = cfg.UuidModule,
                    version = cfg.Version
                }
            };
        }
    }
    
    public class Header
    {
        public string name { get; set; }
        public string description { get; set; }
        public string uuid { get; set; }
        public int[] version { get; set; }
    }

    public class Module
    {
        public string description { get; set; }
        public string type { get; set; }
        public string uuid { get; set; }
        public int[] version { get; set; }
    }
}