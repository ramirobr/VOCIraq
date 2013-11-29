
namespace Cotecna.Voc.Business
{
    public partial class Certificate
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
       

        public bool CanBePublished { 
            get {
            return (WorkflowStatusId == WorkflowStatusEnum.Approved || WorkflowStatusId == WorkflowStatusEnum.Ongoing
                    || WorkflowStatusId == WorkflowStatusEnum.Closed || CertificateStatusId == CertificateStatusEnum.Cancelled);
        } }

        public bool CanBeEdited
        {
            get
            {
                return (WorkflowStatusId == WorkflowStatusEnum.Created || WorkflowStatusId == WorkflowStatusEnum.Rejected
                       || WorkflowStatusId == WorkflowStatusEnum.Requested);
            }
        }

        public bool CanBeEditedCoordinator
        {
            get
            {
                return (WorkflowStatusId == WorkflowStatusEnum.Created || WorkflowStatusId == WorkflowStatusEnum.Rejected);
            }
        }

        public bool CanBeApproved
        {
            get
            {
                return CertificateStatusId != CertificateStatusEnum.Cancelled
                       && (WorkflowStatusId == WorkflowStatusEnum.Created || WorkflowStatusId == WorkflowStatusEnum.Requested);
            }
        }

        public bool CanBeRequested
        {
            get
            {
                return CertificateStatusId != CertificateStatusEnum.Cancelled
                       && (WorkflowStatusId == WorkflowStatusEnum.Created || WorkflowStatusId == WorkflowStatusEnum.Rejected);
            }
        }

        public bool CanBeRejected
        {
            get
            {
                return CertificateStatusId != CertificateStatusEnum.Cancelled
                       && WorkflowStatusId == WorkflowStatusEnum.Requested;
            }
        }

        public bool CanBeRecalled
        {
            get
            {
                return CertificateStatusId != CertificateStatusEnum.Cancelled
                       && WorkflowStatusId == WorkflowStatusEnum.Approved;
            }
        }

        public bool CanBeClosed
        {
            get
            {
                return CertificateStatusId != CertificateStatusEnum.Cancelled
                       && WorkflowStatusId == WorkflowStatusEnum.Ongoing;
            }
        }


        public bool CanBeUnclosed
        {
            get
            {
                return WorkflowStatusId == WorkflowStatusEnum.Closed &&
                    CertificateStatusId != CertificateStatusEnum.Cancelled;
            }
        }

        public bool CanBeDeleted
        {
            get
            {
                return WorkflowStatusId == WorkflowStatusEnum.Created
                    ||  WorkflowStatusId == WorkflowStatusEnum.Rejected
                    ||  WorkflowStatusId == WorkflowStatusEnum.Requested;
            }
        }

        public bool IsCancelled
        {
            get
            {
                return (CertificateStatusId == CertificateStatusEnum.Cancelled);
            }
        }

        public bool IsNCR
        {
            get
            {
                return (CertificateStatusId == CertificateStatusEnum.NonConform);
            }
        }

    }
}
