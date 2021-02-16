namespace Ermes.Social.Dto
{
    public class GetLabelsInput
    {
        public GetLabelsInput()
        {
            Filters = new LabelFilters();
        }
        public LabelFilters Filters { get; set; }
    }
}
