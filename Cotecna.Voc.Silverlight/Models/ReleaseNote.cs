using Cotecna.Voc.Silverlight;

namespace Cotecna.Voc.Business
{
    public partial class ReleaseNote : IEditableDataComparer<ReleaseNote>, ICloneable<ReleaseNote>
    {
        private bool _checked;
        public bool Checked
        {
            get
            {
                return this._checked;
            }
            set
            {
                if ((this._checked != value))
                {
                    this._checked = value;
                    RaisePropertyChanged("Checked");
                }
            }
        }

        public bool Equals(ReleaseNote other)
        {
            bool different = Goods != other.Goods ||
                             NumberOfContainers != other.NumberOfContainers ||
                             Containers != other.Containers ||
                            NetWeight != other.NetWeight ||
                            DocumentaryCheckResultId != other.DocumentaryCheckResultId ||
                            PhysicalCheckResultId != other.PhysicalCheckResultId ||
                            VisualInspectionMade != other.VisualInspectionMade ||
                            OverallResultId != other.OverallResultId ||
                            NoteIssuedId != other.NoteIssuedId ||
                            IssuanceDate != other.IssuanceDate ||
                            JointSamplingMade != other.JointSamplingMade ||
                            IsDeleted != other.IsDeleted ||
                            ImporterName != other.ImporterName ||
                            VisuallyCheck != other.VisuallyCheck ||
                            ContainersDetails != other.ContainersDetails ||
                            ImportDocumentDetails != other.ImportDocumentDetails ||
                            PartialComplete != other.PartialComplete ||
                            NumberLineItems != other.NumberLineItems ||
                            Comments != other.Comments ||
                            Unit != other.Unit ||
                            ShipmentType != other.ShipmentType ||
                            ReceivedQuantity != other.ReceivedQuantity ||
                            RemainingQuantity != other.RemainingQuantity;

            return different ? false : true;
        }

        public void Clone(ReleaseNote xSource)
        {
            Goods = xSource.Goods;
            NumberOfContainers = xSource.NumberOfContainers;
            Containers = xSource.Containers;
            NetWeight = xSource.NetWeight;
            DocumentaryCheckResultId = xSource.DocumentaryCheckResultId;
            PhysicalCheckResultId = xSource.PhysicalCheckResultId;
            VisualInspectionMade = xSource.VisualInspectionMade;
            OverallResultId = xSource.OverallResultId;
            NoteIssuedId = xSource.NoteIssuedId;
            IssuanceDate = xSource.IssuanceDate;
            JointSamplingMade = xSource.JointSamplingMade;
            IsDeleted = xSource.IsDeleted;
            ImporterName = xSource.ImporterName;
            VisuallyCheck = xSource.VisuallyCheck;
            ContainersDetails = xSource.ContainersDetails;
            ImportDocumentDetails = xSource.ImportDocumentDetails;
            PartialComplete = xSource.PartialComplete;
            NumberLineItems = xSource.NumberLineItems;
            Comments = xSource.Comments;
            Unit = xSource.Unit;
            ShipmentType = xSource.ShipmentType;
            ReceivedQuantity = xSource.ReceivedQuantity;
            RemainingQuantity = xSource.RemainingQuantity;
        }

        public Certificate Certificate { get; set; }
    }
}
