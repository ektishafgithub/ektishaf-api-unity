public static class EktishafNftCollection
{
		public const string ERC1155InsufficientBalance_4_Address_Uint256_Uint256_Uint256 = "error ERC1155InsufficientBalance(address,uint256,uint256,uint256)";
		public const string ERC1155InvalidApprover_1_Address = "error ERC1155InvalidApprover(address)";
		public const string ERC1155InvalidArrayLength_2_Uint256_Uint256 = "error ERC1155InvalidArrayLength(uint256,uint256)";
		public const string ERC1155InvalidOperator_1_Address = "error ERC1155InvalidOperator(address)";
		public const string ERC1155InvalidReceiver_1_Address = "error ERC1155InvalidReceiver(address)";
		public const string ERC1155InvalidSender_1_Address = "error ERC1155InvalidSender(address)";
		public const string ERC1155MissingApprovalForAll_2_Address_Address = "error ERC1155MissingApprovalForAll(address,address)";
		public const string EnforcedPause_0_ = "error EnforcedPause()";
		public const string ExpectedPause_0_ = "error ExpectedPause()";
		public const string OwnableInvalidOwner_1_Address = "error OwnableInvalidOwner(address)";
		public const string OwnableUnauthorizedAccount_1_Address = "error OwnableUnauthorizedAccount(address)";
		public const string ApprovalForAll_3_Address_Address_Bool = "event ApprovalForAll(address indexed,address indexed,bool)";
		public const string OwnershipTransferred_2_Address_Address = "event OwnershipTransferred(address indexed,address indexed)";
		public const string Paused_1_Address = "event Paused(address)";
		public const string TransferBatch_5_Address_Address_Address_Uint256Array_Uint256Array = "event TransferBatch(address indexed,address indexed,address indexed,uint256[],uint256[])";
		public const string TransferSingle_5_Address_Address_Address_Uint256_Uint256 = "event TransferSingle(address indexed,address indexed,address indexed,uint256,uint256)";
		public const string URI_2_String_Uint256 = "event URI(string,uint256 indexed)";
		public const string Unpaused_1_Address = "event Unpaused(address)";
		public const string balanceOf_2_Address_Uint256 = "function balanceOf(address,uint256) view returns (uint256)";
		public const string balanceOfBatch_2_AddressArray_Uint256Array = "function balanceOfBatch(address[],uint256[]) view returns (uint256[])";
		public const string burn_3_Address_Uint256_Uint256 = "function burn(address,uint256,uint256)";
		public const string burnBatch_3_Address_Uint256Array_Uint256Array = "function burnBatch(address,uint256[],uint256[])";
		public const string contractURI_0_ = "function contractURI() pure returns (string)";
		public const string exists_1_Uint256 = "function exists(uint256) view returns (bool)";
		public const string getNfts_0_ = "function getNfts() view returns ((uint256,uint256,string)[])";
		public const string isApprovedForAll_2_Address_Address = "function isApprovedForAll(address,address) view returns (bool)";
		public const string mint_4_Address_Uint256_Uint256_String = "function mint(address,uint256,uint256,string)";
		public const string mintBatch_4_Address_Uint256Array_Uint256Array_StringArray = "function mintBatch(address,uint256[],uint256[],string[])";
		public const string owner_0_ = "function owner() view returns (address)";
		public const string pause_0_ = "function pause()";
		public const string paused_0_ = "function paused() view returns (bool)";
		public const string renounceOwnership_0_ = "function renounceOwnership()";
		public const string safeBatchTransferFrom_5_Address_Address_Uint256Array_Uint256Array_Bytes = "function safeBatchTransferFrom(address,address,uint256[],uint256[],bytes)";
		public const string safeTransferFrom_5_Address_Address_Uint256_Uint256_Bytes = "function safeTransferFrom(address,address,uint256,uint256,bytes)";
		public const string setApprovalForAll_2_Address_Bool = "function setApprovalForAll(address,bool)";
		public const string setTokenUri_2_Uint256_String = "function setTokenUri(uint256,string)";
		public const string setTokenUriBatch_2_Uint256Array_StringArray = "function setTokenUriBatch(uint256[],string[])";
		public const string setURI_1_String = "function setURI(string)";
		public const string supportsInterface_1_Bytes4 = "function supportsInterface(bytes4) view returns (bool)";
		public const string totalSupply_0_ = "function totalSupply() view returns (uint256)";
		public const string totalSupply_1_Uint256 = "function totalSupply(uint256) view returns (uint256)";
		public const string transferOwnership_1_Address = "function transferOwnership(address)";
		public const string unpause_0_ = "function unpause()";
		public const string uri_1_Uint256 = "function uri(uint256) view returns (string)";

		public const string Address = "0x52fa3dEFa9358E9164a5fc5528C31351210E3039";
		public const string ABI = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"initialOwner\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"balance\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"needed\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ERC1155InsufficientBalance\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"approver\",\"type\":\"address\"}],\"name\":\"ERC1155InvalidApprover\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"idsLength\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"valuesLength\",\"type\":\"uint256\"}],\"name\":\"ERC1155InvalidArrayLength\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"ERC1155InvalidOperator\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"receiver\",\"type\":\"address\"}],\"name\":\"ERC1155InvalidReceiver\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"ERC1155InvalidSender\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"ERC1155MissingApprovalForAll\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"EnforcedPause\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ExpectedPause\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"OwnableInvalidOwner\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"OwnableUnauthorizedAccount\",\"type\":\"error\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"Paused\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"values\",\"type\":\"uint256[]\"}],\"name\":\"TransferBatch\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"TransferSingle\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"value\",\"type\":\"string\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"URI\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"Unpaused\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address[]\",\"name\":\"accounts\",\"type\":\"address[]\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"}],\"name\":\"balanceOfBatch\",\"outputs\":[{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"burn\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"values\",\"type\":\"uint256[]\"}],\"name\":\"burnBatch\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"contractURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"exists\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getNfts\",\"outputs\":[{\"components\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"uri\",\"type\":\"string\"}],\"internalType\":\"struct EktishafNftCollection.NFT[]\",\"name\":\"\",\"type\":\"tuple[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"uri_\",\"type\":\"string\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"amounts\",\"type\":\"uint256[]\"},{\"internalType\":\"string[]\",\"name\":\"uris_\",\"type\":\"string[]\"}],\"name\":\"mintBatch\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"pause\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"paused\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"values\",\"type\":\"uint256[]\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeBatchTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"uri_\",\"type\":\"string\"}],\"name\":\"setTokenUri\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"internalType\":\"string[]\",\"name\":\"uris_\",\"type\":\"string[]\"}],\"name\":\"setTokenUriBatch\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"newuri\",\"type\":\"string\"}],\"name\":\"setURI\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"unpause\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"uri\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";
		public const string HBI = @"[
  ""constructor(address)"",
  ""error ERC1155InsufficientBalance(address,uint256,uint256,uint256)"",
  ""error ERC1155InvalidApprover(address)"",
  ""error ERC1155InvalidArrayLength(uint256,uint256)"",
  ""error ERC1155InvalidOperator(address)"",
  ""error ERC1155InvalidReceiver(address)"",
  ""error ERC1155InvalidSender(address)"",
  ""error ERC1155MissingApprovalForAll(address,address)"",
  ""error EnforcedPause()"",
  ""error ExpectedPause()"",
  ""error OwnableInvalidOwner(address)"",
  ""error OwnableUnauthorizedAccount(address)"",
  ""event ApprovalForAll(address indexed,address indexed,bool)"",
  ""event OwnershipTransferred(address indexed,address indexed)"",
  ""event Paused(address)"",
  ""event TransferBatch(address indexed,address indexed,address indexed,uint256[],uint256[])"",
  ""event TransferSingle(address indexed,address indexed,address indexed,uint256,uint256)"",
  ""event URI(string,uint256 indexed)"",
  ""event Unpaused(address)"",
  ""function balanceOf(address,uint256) view returns (uint256)"",
  ""function balanceOfBatch(address[],uint256[]) view returns (uint256[])"",
  ""function burn(address,uint256,uint256)"",
  ""function burnBatch(address,uint256[],uint256[])"",
  ""function contractURI() pure returns (string)"",
  ""function exists(uint256) view returns (bool)"",
  ""function getNfts() view returns ((uint256,uint256,string)[])"",
  ""function isApprovedForAll(address,address) view returns (bool)"",
  ""function mint(address,uint256,uint256,string)"",
  ""function mintBatch(address,uint256[],uint256[],string[])"",
  ""function owner() view returns (address)"",
  ""function pause()"",
  ""function paused() view returns (bool)"",
  ""function renounceOwnership()"",
  ""function safeBatchTransferFrom(address,address,uint256[],uint256[],bytes)"",
  ""function safeTransferFrom(address,address,uint256,uint256,bytes)"",
  ""function setApprovalForAll(address,bool)"",
  ""function setTokenUri(uint256,string)"",
  ""function setTokenUriBatch(uint256[],string[])"",
  ""function setURI(string)"",
  ""function supportsInterface(bytes4) view returns (bool)"",
  ""function totalSupply() view returns (uint256)"",
  ""function totalSupply(uint256) view returns (uint256)"",
  ""function transferOwnership(address)"",
  ""function unpause()"",
  ""function uri(uint256) view returns (string)""
]";
}
