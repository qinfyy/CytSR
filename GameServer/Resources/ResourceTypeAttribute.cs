namespace GameServer.Resources
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ResourceTypeAttribute : Attribute
    {
        public List<string> FileName { get; private set; }

        public ResourceTypeAttribute(string fileName, bool isMultifile = false)
        {
            if (isMultifile)
            {
                FileName = new List<string>(fileName.Split(','));
            }
            else
            {
                FileName = [fileName];
            }
        }
    }
}
