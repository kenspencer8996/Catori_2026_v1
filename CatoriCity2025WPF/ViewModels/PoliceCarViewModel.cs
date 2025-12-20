using CatoriCity2025WPF.Objects;
using CatoriServices.Objects.Entities;
using System.ComponentModel;
namespace CatoriCity2025WPF.ViewModels
{
	public class PoliceCarViewModel
	{
		private int _PoliceCarId ;
		private string _CarName ;
		private string _ImageName ;
		private string _CarType ;

		PoliceCarEntity _entity;
	public void EntityToModel(PoliceCarEntity entity)
	{
			_PoliceCarId = entity.PoliceCarId;
		_CarName = entity.CarName;
		_ImageName = entity.ImageName;
		_CarType = entity.CarType;
;
	}
	public int PoliceCarId
	{
		get => _entity.PoliceCarId;
		set
		{
			if (_entity.PoliceCarId != value)
			{
				_entity.PoliceCarId = value;
				OnPropertyChanged(nameof(PoliceCarId));
			}
		}
	}

	public string CarName
	{
		get => _entity.CarName;
		set
		{
			if (_entity.CarName != value)
			{
				_entity.CarName = value;
				OnPropertyChanged(nameof(CarName));
			}
		}
	}

	public string ImageName
	{
		get => _entity.ImageName;
		set
		{
			if (_entity.ImageName != value)
			{
				_entity.ImageName = value;
				OnPropertyChanged(nameof(ImageName));
			}
		}
	}

	public string CarType
	{
		get => _entity.CarType;
		set
		{
			if (_entity.CarType != value)
			{
				_entity.CarType = value;
				OnPropertyChanged(nameof(CarType));
			}
		}
	}


	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged(string propertyName)
	{
 		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
}