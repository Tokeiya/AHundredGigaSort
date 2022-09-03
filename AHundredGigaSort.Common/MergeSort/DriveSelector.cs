namespace AHundredGigaSort.Common.MergeSort;

internal struct DriveSelector
{
	private readonly string _pathA;
	private readonly string _pathB;
	private bool _flg = false;

	public DriveSelector(string pathA, string pathB)
	{
		_pathA = pathA;
		_pathB = pathB;
	}

	public string GetPath()
	{
		if (_flg)
		{
			_flg = false;
			return _pathA;
		}

		_flg = true;
		return _pathB;
	}
}