﻿namespace Sudoku.Bot.Communication;

/// <summary>
/// The table of status codes.
/// </summary>
public static class StatusCodes
{
	/// <summary>
	/// The status code referencing table.
	/// </summary>
	public static readonly IReadOnlyDictionary<int, string> OpenapiCode = new Dictionary<int, string>
	{
        //{ 100, StringResource.Get("Status100")! },
        //{ 101, StringResource.Get("Status101")! },
        { 200, StringResource.Get("Status200")! },
		{ 201, StringResource.Get("Status201")! },
		{ 202, StringResource.Get("Status202")! },
		{ 203, StringResource.Get("Status203")! },
		{ 204, StringResource.Get("Status204")! },
		{ 205, StringResource.Get("Status205")! },
		{ 206, StringResource.Get("Status206")! },
		{ 300, StringResource.Get("Status300")! },
		{ 301, StringResource.Get("Status301")! },
		{ 302, StringResource.Get("Status302")! },
		{ 303, StringResource.Get("Status303")! },
		{ 304, StringResource.Get("Status304")! },
		{ 305, StringResource.Get("Status305")! },
		{ 307, StringResource.Get("Status307")! },
		{ 400, StringResource.Get("Status400")! },
		{ 401, StringResource.Get("Status401")! },
		{ 403, StringResource.Get("Status403")! },
		{ 404, StringResource.Get("Status404")! },
		{ 405, StringResource.Get("Status405")! },
		{ 406, StringResource.Get("Status406")! },
		{ 407, StringResource.Get("Status407")! },
		{ 408, StringResource.Get("Status408")! },
		{ 409, StringResource.Get("Status409")! },
		{ 410, StringResource.Get("Status410")! },
		{ 411, StringResource.Get("Status411")! },
		{ 429, StringResource.Get("Status429")! },
		{ 500, StringResource.Get("Status500")! },
		{ 501, StringResource.Get("Status501")! },
		{ 502, StringResource.Get("Status502")! },
		{ 503, StringResource.Get("Status503")! },
		{ 504, StringResource.Get("Status504")! },
		{ 505, StringResource.Get("Status505")! },
		{ 4001, StringResource.Get("Status4001")! },
		{ 4002, StringResource.Get("Status4002")! },
		{ 4006, StringResource.Get("Status4006")! },
		{ 4007, StringResource.Get("Status4007")! },
		{ 4008, StringResource.Get("Status4008")! },
		{ 4009, StringResource.Get("Status4009")! },
		{ 4010, StringResource.Get("Status4010")! },
		{ 4011, StringResource.Get("Status4011")! },
		{ 4012, StringResource.Get("Status4012")! },
		{ 4013, StringResource.Get("Status4013")! },
		{ 4014, StringResource.Get("Status4014")! },
		{ 4900, StringResource.Get("Status4900")! },
		{ 4914, StringResource.Get("Status4914")! },
		{ 4915, StringResource.Get("Status4915")! },
		{ 10001, StringResource.Get("Status10001")! },
		{ 10003, StringResource.Get("Status10003")! },
		{ 10004, StringResource.Get("Status10004")! },
		{ 11281, StringResource.Get("Status11281")! },
		{ 11282, StringResource.Get("Status11282")! },
		{ 11251, StringResource.Get("Status11251")! },
		{ 11252, StringResource.Get("Status11252")! },
		{ 11253, StringResource.Get("Status11253")! },
		{ 11254, StringResource.Get("Status11254")! },
		{ 11261, StringResource.Get("Status11261")! },
		{ 11262, StringResource.Get("Status11262")! },
		{ 11263, StringResource.Get("Status11263")! },
		{ 11264, StringResource.Get("Status11264")! },
		{ 11265, StringResource.Get("Status11265")! },
		{ 11241, StringResource.Get("Status11241")! },
		{ 11242, StringResource.Get("Status11242")! },
		{ 11243, StringResource.Get("Status11243")! },
		{ 11273, StringResource.Get("Status11273")! },
		{ 11274, StringResource.Get("Status11274")! },
		{ 11275, StringResource.Get("Status11275")! },
		{ 12001, StringResource.Get("Status12001")! },
		{ 12002, StringResource.Get("Status12002")! },
		{ 12003, StringResource.Get("Status12003")! },
		{ 20028, StringResource.Get("Status20028")! },
		{ 50006, StringResource.Get("Status50006")! },
		{ 50035, StringResource.Get("Status50035")! },
		{ 50037, StringResource.Get("Status50037")! },
		{ 50038, StringResource.Get("Status50038")! },
		{ 50039, StringResource.Get("Status50039")! },
		{ 50040, StringResource.Get("Status50040")! },
		{ 50041, StringResource.Get("Status50041")! },
		{ 50042, StringResource.Get("Status50042")! },
		{ 50043, StringResource.Get("Status50043")! },
		{ 50045, StringResource.Get("Status50045")! },
		{ 50046, StringResource.Get("Status50046")! },
		{ 50047, StringResource.Get("Status50047")! },
		{ 301000, StringResource.Get("Status301000")! },
		{ 301001, StringResource.Get("Status301001")! },
		{ 301002, StringResource.Get("Status301002")! },
		{ 301003, StringResource.Get("Status301003")! },
		{ 301004, StringResource.Get("Status301004")! },
		{ 301005, StringResource.Get("Status301005")! },
		{ 301006, StringResource.Get("Status301006")! },
		{ 301007, StringResource.Get("Status301007")! },
		{ 302000, StringResource.Get("Status302000")! },
		{ 302001, StringResource.Get("Status302001")! },
		{ 302002, StringResource.Get("Status302002")! },
		{ 302003, StringResource.Get("Status302003")! },
		{ 302004, StringResource.Get("Status302004")! },
		{ 302005, StringResource.Get("Status302005")! },
		{ 302006, StringResource.Get("Status302006")! },
		{ 302007, StringResource.Get("Status302007")! },
		{ 302008, StringResource.Get("Status302008")! },
		{ 302009, StringResource.Get("Status302009")! },
		{ 302010, StringResource.Get("Status302010")! },
		{ 302011, StringResource.Get("Status302011")! },
		{ 302012, StringResource.Get("Status302012")! },
		{ 302013, StringResource.Get("Status302013")! },
		{ 302014, StringResource.Get("Status302014")! },
		{ 302015, StringResource.Get("Status302015")! },
		{ 302016, StringResource.Get("Status302016")! },
		{ 302017, StringResource.Get("Status302017")! },
		{ 302018, StringResource.Get("Status302018")! },
		{ 302019, StringResource.Get("Status302019")! },
		{ 302020, StringResource.Get("Status302020")! },
		{ 302021, StringResource.Get("Status302021")! },
		{ 302022, StringResource.Get("Status302022")! },
		{ 302023, StringResource.Get("Status302023")! },
		{ 302024, StringResource.Get("Status302024")! },
		{ 502000, StringResource.Get("Status502000")! },
		{ 502001, StringResource.Get("Status502001")! },
		{ 502002, StringResource.Get("Status502002")! },
		{ 502003, StringResource.Get("Status502003")! },
		{ 502004, StringResource.Get("Status502004")! },
		{ 502005, StringResource.Get("Status502005")! },
		{ 502006, StringResource.Get("Status502006")! },
		{ 502007, StringResource.Get("Status502007")! },
		{ 502008, StringResource.Get("Status502008")! },
		{ 502009, StringResource.Get("Status502009")! },
		{ 502010, StringResource.Get("Status502010")! },
		{ 304003, StringResource.Get("Status304003")! },
		{ 304004, StringResource.Get("Status304004")! },
		{ 304005, StringResource.Get("Status304005")! },
		{ 304006, StringResource.Get("Status304006")! },
		{ 304007, StringResource.Get("Status304007")! },
		{ 304008, StringResource.Get("Status304008")! },
		{ 304009, StringResource.Get("Status304009")! },
		{ 304010, StringResource.Get("Status304010")! },
		{ 304011, StringResource.Get("Status304011")! },
		{ 304012, StringResource.Get("Status304012")! },
		{ 304014, StringResource.Get("Status304014")! },
		{ 304016, StringResource.Get("Status304016")! },
		{ 304017, StringResource.Get("Status304017")! },
		{ 304018, StringResource.Get("Status304018")! },
		{ 304019, StringResource.Get("Status304019")! },
		{ 304020, StringResource.Get("Status304020")! },
		{ 304021, StringResource.Get("Status304021")! },
		{ 304022, StringResource.Get("Status304022")! },
		{ 304023, StringResource.Get("Status304023")! },
		{ 304024, StringResource.Get("Status304024")! },
		{ 304025, StringResource.Get("Status304025")! },
		{ 304026, StringResource.Get("Status304026")! },
		{ 304027, StringResource.Get("Status304027")! },
		{ 304028, StringResource.Get("Status304028")! },
		{ 304029, StringResource.Get("Status304029")! },
		{ 304030, StringResource.Get("Status304030")! },
		{ 306001, StringResource.Get("Status306001")! },
		{ 306002, StringResource.Get("Status306002")! },
		{ 306003, StringResource.Get("Status306003")! },
		{ 306004, StringResource.Get("Status306004")! },
		{ 306005, StringResource.Get("Status306005")! },
		{ 306006, StringResource.Get("Status306006")! },
		{ 500000, StringResource.Get("Status500000")! },
		{ 501001, StringResource.Get("Status501001")! },
		{ 501002, StringResource.Get("Status501002")! },
		{ 501003, StringResource.Get("Status501003")! },
		{ 501004, StringResource.Get("Status501004")! },
		{ 501005, StringResource.Get("Status501005")! },
		{ 501006, StringResource.Get("Status501006")! },
		{ 501007, StringResource.Get("Status501007")! },
		{ 501008, StringResource.Get("Status501008")! },
		{ 610000, StringResource.Get("Status610000")! },
		{ 610001, StringResource.Get("Status610001")! },
		{ 610002, StringResource.Get("Status610002")! },
		{ 610003, StringResource.Get("Status610003")! },
		{ 610004, StringResource.Get("Status610004")! },
		{ 610005, StringResource.Get("Status610005")! },
		{ 610006, StringResource.Get("Status610006")! },
		{ 610007, StringResource.Get("Status610007")! },
		{ 610008, StringResource.Get("Status610008")! },
		{ 610009, StringResource.Get("Status610009")! },
		{ 610010, StringResource.Get("Status610010")! },
		{ 610011, StringResource.Get("Status610011")! },
		{ 610012, StringResource.Get("Status610012")! },
		{ 610013, StringResource.Get("Status610013")! },
		{ 610014, StringResource.Get("Status610014")! },
		{ 1000000, StringResource.Get("Status1000000")! },
		{ 1100100, StringResource.Get("Status1100100")! },
		{ 1100101, StringResource.Get("Status1100101")! },
		{ 1100102, StringResource.Get("Status1100102")! },
		{ 1100103, StringResource.Get("Status1100103")! },
		{ 1100104, StringResource.Get("Status1100104")! },
		{ 1100300, StringResource.Get("Status1100300")! },
		{ 1100301, StringResource.Get("Status1100301")! },
		{ 1100302, StringResource.Get("Status1100302")! },
		{ 1100303, StringResource.Get("Status1100303")! },
		{ 1100304, StringResource.Get("Status1100304")! },
		{ 1100305, StringResource.Get("Status1100305")! },
		{ 1100306, StringResource.Get("Status1100306")! },
		{ 1100307, StringResource.Get("Status1100307")! },
		{ 1100308, StringResource.Get("Status1100308")! },
		{ 1100499, StringResource.Get("Status1100499")! },
	};
}