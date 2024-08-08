using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;
using static System.Net.WebRequestMethods;

namespace WayBeyond.UX.Reporting.TexasTech
{
    public class TexasTechViewModel : BindableBase
    {
        private IBeyondRepository _repo;
        private ITransfer _transfer;
        private TexasTechService _service;
        public TexasTechViewModel(ITTRepo db, IBeyondRepository repo, ITransfer transfer)
        {
            _repo = repo;
            _transfer = transfer;
            Upload = new RelayCommand(OnUpload);
            Download = new RelayCommand(OnDownload);
            _service = new TexasTechService(db);
            _transfer = transfer;
        }


        public RelayCommand Upload {  get; private set; }
        public RelayCommand Download { get; private set; }

        private async void OnUpload()
        {
            string reportFolder = $@"{_repo.GetFileLocationByNameAsync(LocationName.TexasTechMonthlyOutput).Result.Path}" +
                                    $@"{_service.ReportMonth} {_service.ReportYear}\";
            _service.CreateFolders(reportFolder);
            _service.TruncateTables();
            string[] files = await _transfer.GetNewFiles(reportFolder);
            _service.ReadActiveRecords($@"{reportFolder}{files.Where(f => f.StartsWith("TT_ACTIVE_INV_new")).FirstOrDefault()}");
            _service.ReadInActiveRecords($@"{reportFolder}{files.Where(f => f.StartsWith("TT_CANCELLED_PIF")).FirstOrDefault()}");
            _service.UpdateDatabase();
            _service.GetTransunionList(@$"{reportFolder}ToTransunion_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv");
            _service.GetPIFList(reportFolder);
        }

        private async void OnDownload()
        {
            string reportFolder = $@"{_repo.GetFileLocationByNameAsync(Data.Models.LocationName.TexasTechMonthlyOutput).Result.Path}" +
                                    $@"{_service.ReportMonth} {_service.ReportYear}\";
            //no need to create folder here they should have already been done.
            _service.UpdateScode(Directory.GetFiles($"{reportFolder}").Where(f => Path.GetFileName(f).StartsWith("From")).FirstOrDefault());
            _service.UpdateTUResults();
            _service.UpdateExpiredAccounts();
            _service.GetBadDebtList(reportFolder);
            _service.GetCharityList(reportFolder);
            _service.GetInventoryList(reportFolder);
            _service.GetCancelList(reportFolder);
        }

    }
}
