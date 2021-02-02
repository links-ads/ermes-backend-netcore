namespace Ermes.Social.Dto
{
    public class GetAnnotationsInput 
    {
        public GetAnnotationsInput()
        {
            Filters = new AnnotationFilters();
        }
        public AnnotationFilters Filters { get; set; }
    }
}
