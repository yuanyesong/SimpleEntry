using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
// ***************************************************************
// IBM Confidential
// 
// OCO Source Materials
// 
// IBM SPSS Products: Statistics Common
// 
// (C) Copyright IBM Corp. 1989, 2013
// 
// The source code for this program is not published or otherwise divested of its trade secrets, 
// irrespective of what has been deposited with the U.S. Copyright Office.
// ***************************************************************

static class SpssDioInterface
{

	// Modification history:
	// 16 Sep 96 - SPSS 7.5
	// 05 Dec 97 - SPSS 8.0
	// 27 Aug 98 - SPSS 9.0
	// 02 Sep 99 - SPSS 10.0
	// 29 mar 01 - SPSS 11.0
	// 04 Feb 03 - SPSS 12.0 - Long variable names
	// 27 Aug 04 - SPSS 13.0 - Extended strings
	// 23 Aug 07 - SPSS 16.0 - Unicode
	// 03 Apr 13 - Statistics 22.0 - Password protection

	// Error codes returned by functions

	public const int  SPSS_OK = 0;
	public const int  SPSS_FILE_OERROR = 1;
	public const int  SPSS_FILE_WERROR = 2;
	public const int  SPSS_FILE_RERROR = 3;
	public const int  SPSS_FITAB_FULL = 4;
	public const int  SPSS_INVALID_HANDLE = 5;
	public const int  SPSS_INVALID_FILE = 6;

	public const int  SPSS_NO_MEMORY = 7;
	public const int  SPSS_OPEN_RDMODE = 8;

	public const int  SPSS_OPEN_WRMODE = 9;
	public const int  SPSS_INVALID_VARNAME = 10;
	public const int  SPSS_DICT_EMPTY = 11;
	public const int  SPSS_VAR_NOTFOUND = 12;
	public const int  SPSS_DUP_VAR = 13;
	public const int  SPSS_NUME_EXP = 14;
	public const int  SPSS_STR_EXP = 15;
	public const int  SPSS_SHORTSTR_EXP = 16;

	public const int  SPSS_INVALID_VARTYPE = 17;
	public const int  SPSS_INVALID_MISSFOR = 18;
	public const int  SPSS_INVALID_COMPSW = 19;
	public const int  SPSS_INVALID_PRFOR = 20;
	public const int  SPSS_INVALID_WRFOR = 21;
	public const int  SPSS_INVALID_DATE = 22;

	public const int  SPSS_INVALID_TIME = 23;
	public const int  SPSS_NO_VARIABLES = 24;
	public const int  SPSS_MIXED_TYPES = 25;

	public const int  SPSS_DUP_VALUE = 27;
	public const int  SPSS_INVALID_CASEWGT = 28;
	public const int  SPSS_INCOMPATIBLE_DICT = 29;
	public const int  SPSS_DICT_COMMIT = 30;

	public const int  SPSS_DICT_NOTCOMMIT = 31;
	public const int  SPSS_NO_TYPE2 = 33;
	public const int  SPSS_NO_TYPE73 = 41;
	public const int  SPSS_INVALID_DATEINFO = 45;
	public const int  SPSS_NO_TYPE999 = 46;
	public const int  SPSS_EXC_STRVALUE = 47;
	public const int  SPSS_CANNOT_FREE = 48;
	public const int  SPSS_BUFFER_SHORT = 49;
	public const int  SPSS_INVALID_CASE = 50;
	public const int  SPSS_INTERNAL_VLABS = 51;
	public const int  SPSS_INCOMPAT_APPEND = 52;
	public const int  SPSS_INTERNAL_D_A = 53;
	public const int  SPSS_FILE_BADTEMP = 54;
	public const int  SPSS_DEW_NOFIRST = 55;
	public const int  SPSS_INVALID_MEASURELEVEL = 56;
	public const int  SPSS_INVALID_7SUBTYPE = 57;
	public const int  SPSS_INVALID_VARHANDLE = 58;
	public const int  SPSS_INVALID_ENCODING = 59;

	public const int  SPSS_FILES_OPEN = 60;
	public const int  SPSS_INVALID_MRSETDEF = 70;
	public const int  SPSS_INVALID_MRSETNAME = 71;
	public const int  SPSS_DUP_MRSETNAME = 72;
	public const int  SPSS_BAD_EXTENSION = 73;
	public const int  SPSS_INVALID_EXTENDEDSTRING = 74;
	public const int  SPSS_INVALID_ATTRNAME = 75;
	public const int  SPSS_INVALID_ATTRDEF = 76;
	public const int  SPSS_INVALID_MRSETINDEX = 77;
	public const int  SPSS_INVALID_VARSETDEF = 78;

	public const int  SPSS_INVALID_ROLE = 79;
	// Warning codes returned by functions
	public const int  SPSS_EXC_LEN64 = -1;
	public const int  SPSS_EXC_LEN120 = -2;
	public const int  SPSS_EXC_VARLABEL = -2;
	public const int  SPSS_EXC_LEN60 = -4;
	public const int  SPSS_EXC_VALLABEL = -4;
	public const int  SPSS_FILE_END = -5;
	public const int  SPSS_NO_VARSETS = -6;
	public const int  SPSS_EMPTY_VARSETS = -7;
	public const int  SPSS_NO_LABELS = -8;
	public const int  SPSS_NO_LABEL = -9;
	public const int  SPSS_NO_CASEWGT = -10;
	public const int  SPSS_NO_DATEINFO = -11;
	public const int  SPSS_NO_MULTRESP = -12;
	public const int  SPSS_EMPTY_MULTRESP = -13;
	public const int  SPSS_NO_DEW = -14;

	public const int  SPSS_EMPTY_DEW = -15;

	// Missing value format codes
	public const int SPSS_NO_MISSVAL = 0;
	public const int SPSS_ONE_MISSVAL = 1;
	public const int SPSS_TWO_MISSVAL = 2;
	public const int SPSS_THREE_MISSVAL = 3;
	public const int SPSS_MISS_RANGE = -2;

	public const int SPSS_MISS_RANGEANDVAL = -3;


	// SPSS Format Type Codes
		// Alphanumeric
	public const int SPSS_FMT_A = 1;
		// Alphanumeric hexadecimal
	public const int SPSS_FMT_AHEX = 2;
		// F Format with commas
	public const int SPSS_FMT_COMMA = 3;
		// Commas and floating dollar sign
	public const int SPSS_FMT_DOLLAR = 4;
		// Default Numeric Format
	public const int SPSS_FMT_F = 5;
		// Integer binary
	public const int SPSS_FMT_IB = 6;
		// Positive integer binary - hex
	public const int SPSS_FMT_PIBHEX = 7;
		// Packed decimal
	public const int SPSS_FMT_P = 8;
		// Positive integer binary unsigned
	public const int SPSS_FMT_PIB = 9;
		// Positive integer binary unsigned
	public const int SPSS_FMT_PK = 10;
		// Floating point binary
	public const int SPSS_FMT_RB = 11;
		// Floating point binary hex
	public const int SPSS_FMT_RBHEX = 12;
		// Zoned decimal
	public const int SPSS_FMT_Z = 15;
		// N Format- unsigned with leading 0s
	public const int SPSS_FMT_N = 16;
		// E Format- with explicit power of 10
	public const int SPSS_FMT_E = 17;
		// Date format dd-mmm-yyyy
	public const int SPSS_FMT_DATE = 20;
		// Time format hh:mm:ss.s
	public const int SPSS_FMT_TIME = 21;
		// Date and Time
	public const int SPSS_FMT_DATE_TIME = 22;
		// Date format dd-mmm-yyyy
	public const int SPSS_FMT_ADATE = 23;
		// Julian date - yyyyddd
	public const int SPSS_FMT_JDATE = 24;
		// Date-time dd hh:mm:ss.s
	public const int SPSS_FMT_DTIME = 25;
		// Day of the week
	public const int SPSS_FMT_WKDAY = 26;
		// Month
	public const int SPSS_FMT_MONTH = 27;
		// mmm yyyy
	public const int SPSS_FMT_MOYR = 28;
		// q Q yyyy
	public const int SPSS_FMT_QYR = 29;
		// ww WK yyyy
	public const int SPSS_FMT_WKYR = 30;
		// Percent - F followed by %
	public const int SPSS_FMT_PCT = 31;
		// Like COMMA, switching dot for comma
	public const int SPSS_FMT_DOT = 32;
		// User Programmable currency format
	public const int SPSS_FMT_CCA = 33;
		// User Programmable currency format
	public const int SPSS_FMT_CCB = 34;
		// User Programmable currency format
	public const int SPSS_FMT_CCC = 35;
		// User Programmable currency format
	public const int SPSS_FMT_CCD = 36;
		// User Programmable currency format
	public const int SPSS_FMT_CCE = 37;
		// Date in dd/mm/yyyy style
	public const int SPSS_FMT_EDATE = 38;
		// Date in yyyy/mm/dd style
	public const int SPSS_FMT_SDATE = 39;


	// Definitions of "type 7" records
		// Documents (actually type 6
	public const int SPSS_T7_DOCUMENTS = 0;
		// VAX Data Entry - dictionary version
	public const int SPSS_T7_VAXDE_DICT = 1;
		// VAX Data Entry - data
	public const int SPSS_T7_VAXDE_DATA = 2;
		// Source system characteristics
	public const int SPSS_T7_SOURCE = 3;
		// Source system floating pt constants
	public const int SPSS_T7_HARDCONST = 4;
		// Variable sets
	public const int SPSS_T7_VARSETS = 5;
		// Trends date information
	public const int SPSS_T7_TRENDS = 6;
		// Multiple response groups
	public const int SPSS_T7_MULTRESP = 7;
		// Windows Data Entry data
	public const int SPSS_T7_DEW_DATA = 8;
		// TextSmart data
	public const int SPSS_T7_TEXTSMART = 10;
		// Msmt level, col width, & alignment
	public const int SPSS_T7_MSMTLEVEL = 11;
		// Windows Data Entry GUID
	public const int SPSS_T7_DEW_GUID = 12;
		// Extended variable names
	public const int SPSS_T7_XVARNAMES = 13;
		//Extended strings
	public const int SPSS_T7_XSTRINGS = 14;
		//Clementine Metadata
	public const int SPSS_T7_CLEMENTINE = 15;
		//64 bit N of cases
	public const int SPSS_T7_NCASES = 16;
		//File level attributes
	public const int SPSS_T7_FILE_ATTR = 17;
		//Variable attributes
	public const int SPSS_T7_VAR_ATTR = 18;
		// Extended multiple response groups
	public const int SPSS_T7_EXTMRSETS = 19;
		// Encoding, aka code page
	public const int SPSS_T7_ENCODING = 20;
		// Value labels for long strings
	public const int SPSS_T7_LONGSTRLABS = 21;
		// Missing values for long strings
	public const int SPSS_T7_LONGSTRMVAL = 22;

	// Encoding modes
		// Text encoded in current code page
	public const int SPSS_ENCODING_CODEPAGE = 0;
		// Text encoded as UTF-8
	public const int SPSS_ENCODING_UTF8 = 1;


	// Diagnostics regarding SPSS variable names
		// Valid standard name
    public const int  SPSS_NAME_OK = 0;
		// Valid scratch var name
	public const int  SPSS_NAME_SCRATCH = 1;
		// Valid system var name
	public const int  SPSS_NAME_SYSTEM = 2;
		// Empty or longer than SPSS_MAX_VARNAME
	public const int  SPSS_NAME_BADLTH = 3;
		// Invalid character or imbedded blank
	public const int  SPSS_NAME_BADCHAR = 4;
		// Name is a reserved word
	public const int  SPSS_NAME_RESERVED = 5;
		// Invalid initial character (otherwise OK)
	public const int  SPSS_NAME_BADFIRST = 6;

	// Maximum lengths of SPSS data file objects
		// Variable name
	public const int  SPSS_MAX_VARNAME = 64;
		// Short (compatibility) variable name
	public const int  SPSS_MAX_SHORTVARNAME = 8;
		// Short string variable
	public const int  SPSS_MAX_SHORTSTRING = 8;
		// File label string
	public const int  SPSS_MAX_IDSTRING = 64;
		// Long string variable
	public const int  SPSS_MAX_LONGSTRING = 32767;
		// Value label
	public const int  SPSS_MAX_VALLABEL = 120;
		// Variable label
	public const int  SPSS_MAX_VARLABEL = 256;
		// Maximum record 7 subtype
	public const int  SPSS_MAX_7SUBTYPE = 40;
		// Maximum encoding text
	public const int  SPSS_MAX_ENCODING = 64;
	[DllImport("spssio32.dll", EntryPoint = "spssAddFileAttribute@16", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]


	// Functions exported by spssio32.dll in alphabetical order
	public static extern int spssAddFileAttribute(int handle, string attribName, int attribSub, string attribText);
	[DllImport("spssio32.dll", EntryPoint = "spssAddVarAttribute@20", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssAddMultRespDefC Lib "spssio32.dll" Alias "spssAddMultRespDefC@28" _
	//                           (ByVal handle As Integer, ByVal mrSetName As String, ByVal mrSetLabel As String, ByVal isDichotomy As Integer, ByVal countedValue As Integer, ByVal *varNames As String, ByVal numVars As Integer) As Integer
	// Public Declare Function   spssAddMultRespDefExt "spssio32.dll" Alias "spssAddMultRespDefExt@8" _
	//                           (ByVal handle As Integer, ByVal pSet as String) As Integer
	// Public Declare Function   spssAddMultRespDefN Lib "spssio32.dll" Alias "spssAddMultRespDefN@28" _
	//                           (ByVal handle As Integer, ByVal mrSetName As String, ByVal mrSetLabel As String, ByVal isDichotomy As Integer, ByVal countedValue As String, ByVal *varNames As String, ByVal numVars As Integer) As Integer
	public static extern int spssAddVarAttribute(int handle, string varName, string attribName, int attribSub, string attribText);
	[DllImport("spssio32.dll", EntryPoint = "spssCloseAppend@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssCloseAppend(int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssCloseRead@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssCloseRead(int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssCloseWrite@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssCloseWrite(int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssCommitCaseRecord@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssCommitCaseRecord(int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssCommitHeader@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssCommitHeader(int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssConvertDate@16", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssConvertDate(int day, int month, int year, ref double spssDate);
	[DllImport("spssio32.dll", EntryPoint = "spssConvertSPSSDate@20", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssConvertSPSSDate(ref int day, ref int month, ref int year, double spssDate);
	[DllImport("spssio32.dll", EntryPoint = "spssConvertSPSSTime@24", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssConvertSPSSTime(ref int day, ref int hourh, ref int minute, ref double second, double spssDate);
	[DllImport("spssio32.dll", EntryPoint = "spssConvertTime@24", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssConvertTime(int day, int hour, int minute, double second, ref double spssTime);
	[DllImport("spssio32.dll", EntryPoint = "spssCopyDocuments@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssCopyDocuments(int fromHandle, int toHandle);
	[DllImport("spssio32.dll", EntryPoint = "spssFreeDateVariables@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssFreeAttributes Lib "spssio32.dll" Alias "spssFreeAttributes@12" _
	//                           (ByRef *attribNames As String, ByRef *attribText As String, ByVal nAttributes) As Integer
	public static extern int spssFreeDateVariables(ref int pDateInfo);
	[DllImport("spssio32.dll", EntryPoint = "spssFreeMultRespDefs@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssFreeMultRespDefs(int pMrespDefs);
	[DllImport("spssio32.dll", EntryPoint = "spssFreeVariableSets@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssFreeMultRespDefStruct Lib "spssio32.dll" Alias "spssFreeMultRespDefStruct@4" _
	//                           (ByVal pSet As String) As Integer
	// Public Declare Function   spssFreeVarCValueLabels Lib "spssio32.dll" Alias "spssFreeVarCValueLabels@12" _
	//                           (ByRef *values As String, ByRef *labels As String, ByVal numLabels As Integer) As Integer
	public static extern int spssFreeVariableSets(int pVarSets);
	[DllImport("spssio32.dll", EntryPoint = "spssGetCaseSize@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssFreeVarNames Lib "spssio32.dll" Alias "spssFreeVarNames@12" _
	//                           (ByVal *varNames As String, ByRef varTypes As Integer, ByVal numVars As Integer) As Integer
	// Public Declare Function   spssFreeVarNValueLabels Lib "spssio32.dll" Alias "spssFreeVarNValueLabels@12" _
	//                           (ByRef values As Double, ByVal *labels As String, ByVal numLabels As Integer) As Integer
	public static extern int spssGetCaseSize(int handle, ref int caseSize);
	[DllImport("spssio32.dll", EntryPoint = "spssGetCaseWeightVar@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetCaseWeightVar(int handle, string varName);
	[DllImport("spssio32.dll", EntryPoint = "spssGetCompression@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetCompression(int handle, ref int compSwitch);
	[DllImport("spssio32.dll", EntryPoint = "spssGetDateVariables@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetDateVariables(int handle, ref int numofElements, ref int pDateInfo);
	[DllImport("spssio32.dll", EntryPoint = "spssGetDEWFirst@16", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetDEWFirst(int handle, string Data, int maxData, ref int data);
	[DllImport("spssio32.dll", EntryPoint = "spssGetDEWGUID@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetDEWGUID(int handle, string asciiGUID);
	[DllImport("spssio32.dll", EntryPoint = "spssGetDEWInfo@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetDEWInfo(int handle, ref int Length, ref int HashTotal);
	[DllImport("spssio32.dll", EntryPoint = "spssGetDEWNext@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetDEWNext(int handle, string Data, int maxData, ref int nData);
	[DllImport("spssio32.dll", EntryPoint = "spssGetEstimatedNofCases@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetEstimatedNofCases(int handle, ref int caseCount);
	[DllImport("spssio32.dll", EntryPoint = "spssGetFileCodePage@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssGetFileAttributes Lib "spssio32.dll" Alias "spssGetFileAttributes@16" _
	//                           (ByVal handle As Integer, ByVal **attribNames As String, ByVal **attribText As String, ByRef nAttributes As Integer) As Integer
	public static extern int spssGetFileCodePage(int handle, ref int nCodePage);
	[DllImport("spssio32.dll", EntryPoint = "spssGetFileEncoding@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetFileEncoding(int handle, string szEncoding);
	[DllImport("spssio32.dll", EntryPoint = "spssGetIdString@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetIdString(int handle, string id);
	[DllImport("spssio32.dll", EntryPoint = "spssGetInterfaceEncoding@0", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetInterfaceEncoding();
	[DllImport("spssio32.dll", EntryPoint = "spssGetMultRespCount@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetMultRespCount(int handle, ref int nSets);
	[DllImport("spssio32.dll", EntryPoint = "spssGetMultRespDefs@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssGetMultRespDefByIndex Lib "spssio32.dll" Alias "spssGetMultRespDefByIndex@12" _
	//                           (ByVal handle As Integer, ByVal iSet As Integer, ByVal *ppSet As String) As Integer
	public static extern int spssGetMultRespDefs(int handle, ref int pMrespDefs);
	[DllImport("spssio32.dll", EntryPoint = "spssGetMultRespDefsEx@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetMultRespDefsEx(int handle, ref int pMrespDefs);
	[DllImport("spssio32.dll", EntryPoint = "spssGetNumberofCases@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetNumberofCases(int handle, ref int caseCount);
	[DllImport("spssio32.dll", EntryPoint = "spssGetNumberofVariables@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetNumberofVariables(int handle, ref int numVars);
	[DllImport("spssio32.dll", EntryPoint = "spssGetReleaseInfo@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetReleaseInfo(int handle, ref int relInfo);
	[DllImport("spssio32.dll", EntryPoint = "spssGetSystemString@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetSystemString(int handle, string sysName);
	[DllImport("spssio32.dll", EntryPoint = "spssGetTextInfo@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetTextInfo(int handle, string textInfo);
	[DllImport("spssio32.dll", EntryPoint = "spssGetTimeStamp@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetTimeStamp(int handle, string fileDate, string fileTime);
	[DllImport("spssio32.dll", EntryPoint = "spssGetValueChar@20", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetValueChar(int handle, double varHandle, string value, int valueSize);
	[DllImport("spssio32.dll", EntryPoint = "spssGetValueNumeric@16", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetValueNumeric(int handle, double varHandle, ref double value);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarAlignment@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetVarAlignment(int handle, string varName, ref int alignment);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarCMissingValues@24", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssGetVarAttributes Lib "spssio32.dll" Alias "spssGetVarAttributes@20" _
	//                           (ByVal handle As Integer, ByVal varName As String, ByVal **attribNames As String, ByVal **attribText As String, ByRef nAttributes As Integer) As Integer
	public static extern int spssGetVarCMissingValues(int handle, string varName, ref int missingFormat, string missingVal1, string missingVal2, string missingVal3);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarColumnWidth@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetVarColumnWidth(int handle, string varName, ref int columnWidth);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarCompatName@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetVarCompatName(int handle, string longName, string shortName);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarCValueLabel@16", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetVarCValueLabel(int handle, string varName, string value, string label);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarCValueLabelLong@24", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetVarCValueLabelLong(int handle, string varName, string value, string labelBuff, int lenBuff, ref int lenLabel);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarHandle@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssGetVarCValueLabels Lib "spssio32.dll" Alias "spssGetVarCValueLabels@20" _
	//                           (ByVal handle As Integer, ByVal varName As String, ByVal **values As String, ByVal **labels As String, ByRef numofLabels As Integer) As Integer
	public static extern int spssGetVarHandle(int handle, string varName, ref double varHandle);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVariableSets@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetVariableSets(int handle, ref int pVarSets);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarInfo@16", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetVarInfo(int handle, int iVar, string varName, ref int varType);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarLabel@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetVarLabel(int handle, string varName, string varLabel);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarLabelLong@20", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetVarLabelLong(int handle, string varName, string labelBuff, int lenBuff, ref int lenLabel);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarMeasureLevel@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetVarMeasureLevel(int handle, string varName, ref int measureLevel);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarRole@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetVarRole(int handle, string varName, ref int varRole);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarNMissingValues@24", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssGetVarNames Lib "spssio32.dll" Alias "spssGetVarNames@16" _
	//                           (ByVal handle As Integer, ByRef numVars As Integer, ByRef **varNames As String, ByRef *varTypes As Integer) As Integer
	public static extern int spssGetVarNMissingValues(int handle, string varName, ref int missingFormat, ref double missingVal1, ref double missingVal2, ref double missingVal3);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarNValueLabel@20", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetVarNValueLabel(int handle, string varName, double value, string label);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarNValueLabelLong@28", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetVarNValueLabelLong(int handle, string varName, double value, string labelBuff, int lenBuff, ref int lenLabel);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarPrintFormat@20", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssGetVarNValueLabels Lib "spssio32.dll" Alias "spssGetVarNValueLabels@20" _
	//                           (ByVal handle As Integer, ByVal varName As String, ByRef *values As Double, ByVal **labels As String, ByRef numofLabels As Integer) As Integer
	public static extern int spssGetVarPrintFormat(int handle, string varName, ref int printType, ref int printDec, ref int printWidth);
	[DllImport("spssio32.dll", EntryPoint = "spssGetVarWriteFormat@20", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssGetVarWriteFormat(int handle, string varName, ref int writeType, ref int writeDec, ref int writeWidth);
	[DllImport("spssio32.dll", EntryPoint = "spssHostSysmisVal@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern void spssHostSysmisVal(ref double missVal);
	[DllImport("spssio32.dll", EntryPoint = "spssIsCompatibleEncoding@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssIsCompatibleEncoding(int handle, ref int bCompatible);
	[DllImport("spssio32.dll", EntryPoint = "spssLowHighVal@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern void spssLowHighVal(ref double lowest, ref double highest);
	[DllImport("spssio32.dll", EntryPoint = "spssOpenAppend@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssOpenAppend(string fileName, ref int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssOpenAppendEx@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssOpenAppendEx(string fileName, string password, ref int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssOpenRead@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssOpenRead(string fileName, ref int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssOpenReadEx@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssOpenReadEx(string fileName, string password, ref int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssOpenWrite@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssOpenWrite(string fileName, ref int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssOpenWriteEx@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssOpenWriteEx(string fileName, string password, ref int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssOpenWriteCopy@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssOpenWriteCopy(string fileName, string dictFileName, ref int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssOpenWriteCopyEx@20", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssOpenWriteCopyEx(string fileName, string password, string dictFileName, string dictPassword, ref int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssOpenWriteCopyExFile@16", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssOpenWriteCopyExFile(string fileName, string password, string dictFileName, ref int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssOpenWriteCopyExDict@16", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssOpenWriteCopyExDict(string fileName, string dictFileName, string dictPassword, ref int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssQueryType7@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssQueryType7(int fromHandle, int subType, ref int bFound);
	[DllImport("spssio32.dll", EntryPoint = "spssReadCaseRecord@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssReadCaseRecord(int handle);
	[DllImport("spssio32.dll", EntryPoint = "spssSeekNextCase@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSeekNextCase(int handle, int caseNumber);
	[DllImport("spssio32.dll", EntryPoint = "spssSetCaseWeightVar@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetCaseWeightVar(int handle, string varName);
	[DllImport("spssio32.dll", EntryPoint = "spssSetCompression@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetCompression(int handle, int compSwitch);
	[DllImport("spssio32.dll", EntryPoint = "spssSetDateVariables@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetDateVariables(int handle, int numofElements, ref int dateInfo);
	[DllImport("spssio32.dll", EntryPoint = "spssSetDEWFirst@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetDEWFirst(int handle, string Data, int nBytes);
	[DllImport("spssio32.dll", EntryPoint = "spssSetDEWGUID@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetDEWGUID(int handle, string asciiGUID);
	[DllImport("spssio32.dll", EntryPoint = "spssSetDEWNext12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetDEWNext(int handle, string Data, int nBytes);
	[DllImport("spssio32.dll", EntryPoint = "spssSetIdString@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssSetFileAttributes Lib "spssio32.dll" Alias "spssSetFileAttributes@16" _
	//                           (ByVal handle As Integer, ByVal *attribNames As String, ByVal *attribText As String, ByVal nAttributes As Integer) As Integer
	public static extern int spssSetIdString(int handle, string id);
	[DllImport("spssio32.dll", EntryPoint = "spssSetInterfaceEncoding@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetInterfaceEncoding(int iEncoding);
	[DllImport("spssio32.dll", EntryPoint = "spssSetMultRespDefs@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssSetLocale Lib "spssio32.dll" Alias "spssSetLocale@8" _
	//                           (ByVal iCategory As Integer, ByVal szLocale As String) As String
	public static extern int spssSetMultRespDefs(int handle, string mrespDefs);
	[DllImport("spssio32.dll", EntryPoint = "spssSetTempDir@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetTempDir(string dirName);
	[DllImport("spssio32.dll", EntryPoint = "spssSetTextInfo@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetTextInfo(int handle, string textInfo);
	[DllImport("spssio32.dll", EntryPoint = "spssSetValueChar@16", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetValueChar(int handle, double varHandle, string value);
	[DllImport("spssio32.dll", EntryPoint = "spssSetValueNumeric@20", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetValueNumeric(int handle, double varHandle, double value);
	[DllImport("spssio32.dll", EntryPoint = "spssSetVarAlignment@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetVarAlignment(int handle, string varName, int alignment);
	[DllImport("spssio32.dll", EntryPoint = "spssSetVarCMissingValues@24", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssSetVarAttributes Lib "spssio32.dll" Alias "spssSetVarAttributes@20" _
	//                           (ByVal handle As Integer, ByVal varName As String, ByVal *attribNames As String, ByVal *attribText As String, ByVal nAttributes As Integer) As Integer
	public static extern int spssSetVarCMissingValues(int handle, string varName, int missingFormat, string missingVal1, string missingVal2, string missingVal3);
	[DllImport("spssio32.dll", EntryPoint = "spssSetVarColumnWidth@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetVarColumnWidth(int handle, string varName, int columnWidth);
	[DllImport("spssio32.dll", EntryPoint = "spssSetVarCValueLabel@16", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetVarCValueLabel(int handle, string varName, string value, string label);
	[DllImport("spssio32.dll", EntryPoint = "spssSetVariableSets@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssSetVarCValueLabels Lib "spssio32.dll" Alias "spssSetVarCValueLabels@20" _
	//                           (ByVal handle As Integer, ByVal *varNames As String, ByVal numofVars As Integer, ByVal *values As String, ByVal *labels As String, ByVal numofLabels As Integer) As Integer
	public static extern int spssSetVariableSets(int handle, string varSets);
	[DllImport("spssio32.dll", EntryPoint = "spssSetVarLabel@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetVarLabel(int handle, string varName, string varLabel);
	[DllImport("spssio32.dll", EntryPoint = "spssSetVarMeasureLevel@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetVarMeasureLevel(int handle, string varName, int measureLevel);
	[DllImport("spssio32.dll", EntryPoint = "spssSetVarRole@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetVarRole(int handle, string varName, int varRole);
	[DllImport("spssio32.dll", EntryPoint = "spssSetVarName@12", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetVarName(int handle, string varName, int varType);
	[DllImport("spssio32.dll", EntryPoint = "spssSetVarNMissingValues@36", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetVarNMissingValues(int handle, string varName, int missingFormat, double missingVal1, double missingVal2, double missingVal3);
	[DllImport("spssio32.dll", EntryPoint = "spssSetVarNValueLabel@20", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetVarNValueLabel(int handle, string varName, double value, string label);
	[DllImport("spssio32.dll", EntryPoint = "spssSetVarPrintFormat@20", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	// Public Declare Function   spssSetVarNValueLabels Lib "spssio32.dll" Alias "spssSetVarNValueLabels@24" _
	//                           (ByVal handle As Integer, ByVal *varNames As String, ByVal numofVars As Integer, ByRef values As Double, ByVal *labels As String, ByVal numofLabels As Integer) As Integer
	public static extern int spssSetVarPrintFormat(int handle, string varName, int printType, int printDec, int printWidth);
	[DllImport("spssio32.dll", EntryPoint = "spssSetVarWriteFormat@20", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssSetVarWriteFormat(int handle, string varName, int writeType, int writeDec, int writeWidth);
	[DllImport("spssio32.dll", EntryPoint = "spssSysmisVal@0", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern double spssSysmisVal();
	[DllImport("spssio32.dll", EntryPoint = "spssValidateVarname@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssValidateVarname(string varName);
	[DllImport("spssio32.dll", EntryPoint = "spssWholeCaseIn@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssWholeCaseIn(int handle, ref double caseRec);
	[DllImport("spssio32.dll", EntryPoint = "spssWholeCaseOut@8", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
	public static extern int spssWholeCaseOut(int handle, ref double caseRec);
}

