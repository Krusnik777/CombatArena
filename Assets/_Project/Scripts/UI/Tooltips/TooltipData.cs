namespace UI.Tooltips
{
    public class TooltipData
    {
        public string Title;
        public string Description;
        public string Info;

        public TooltipData(string title, string description = "", string info = "")
        {
            Title = title;
            Description = description;
            Info = info;
        }
    }
}
