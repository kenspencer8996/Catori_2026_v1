using CatoriCity2025WPF.Objects.Services;
using CatoriCity2025WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriCity2025WPF.Controllers
{
    public class BankEditController
    {
        private readonly BankService _service = new BankService();
        private readonly Views.BankEditView _view;
        public BankViewModel EditModel { get; }

        public BankEditController(Views.BankEditView view, BankViewModel model)
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
