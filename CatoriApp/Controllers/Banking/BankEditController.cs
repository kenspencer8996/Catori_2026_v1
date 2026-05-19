using CatoriApp.Objects.Services;
using System;
using System.Collections.Generic;
using System.Text;
namespace CatoriApp.Controllers.Banking
{
    public class BankEditController
    {
        private readonly BankService _service = new BankService();
        private readonly Views.Controls.Banking.BankEditView _view;
        public BankViewModel EditModel { get; }

        public BankEditController(Views.Controls.Banking.BankEditView view, BankViewModel model)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            EditModel = model ?? new BankViewModel();
        }

        public async Task SaveAsync()
        {
            var entity = EditModel.GetEntity();
            await _service.UpsertAsync(entity);
        }
    }
}




