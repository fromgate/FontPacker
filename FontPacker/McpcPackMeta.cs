namespace FontPacker
{
    public class McpcPackMeta
    {
        public Pack pack { get; set; }

        public McpcPackMeta(Cfg cfg)
        {
            pack = new Pack()
            {
                pack_format = 3,
                description = cfg.Description
            };
        }
        
    }
    
    
    public class Pack
    {
        public int pack_format { get; set; }
        public string description { get; set; }
    }

}