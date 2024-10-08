<h3 align="center">Ektishaf Unity API</h3>

  <p align="center">
     <img src="https://content.pstmn.io/9a4265d8-6283-42c1-b769-b043d3e9af7d/SWNvbiA1MDB4NTAwLnBuZw==" width="64" height="64"><br/>
    This API allows Unity developers to register new in-game wallets (or use an existing wallet), enables them to communicate seamlessly with EVM compatible blockchains and makes it fun to implement NFTs in Unity.<br/>
  </p>
</div>

[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)
<br/>
Before we jump into installing the Unity package, feel free to check our website if you want to implement using REST API.<br/>
_If you have questions, feel free to email us at mail@ektishaf.com_ or check our [official website](https://ektishaf.com) and [documentation](https://ektishaf.com/documentation).
<br/><br/>

> [!IMPORTANT]
> * We never store any private key or password in any database
> * We never ask for any private information from our customers

> [!TIP]
> * Private keys and passwords should never be disclosed anywhere at all other than wherever required by the API.
> * We further suggest to use cryptographic services if you have planned to store any critical response data in a secured PlayerPrefs.

### Welcome! 👋
Welcome to Ektishaf Unity API!.<br/><br/>
If you are a developer, you might be thinking about where to start and in which direction to go to make it work in a game.<br/>
So to make it very simple here are the three things to keep in mind. <br/>

1. **Smart Contract - A verified smart contract in hand that is deployed in any EVM compatible testnet, such as sepolia etc.**
2. **API - An API that communicates with both smart contract (blockchain) and game.**
3. **Game - An ongoing game project that supports API calls.**

Please read below about the smart contract and how to install the API package to get started with the core functionalities right away.
<br/><br/>

### Smart Contract
Our API provides you with a sample smart contract that is tested several times to make sure there are no loose points in terms of security.
You can later on copy the code and deploy as your own smart contract on whichever EVM blockchain you prefer.
For now, our sample verified smart contract is deployed on sepolia (testnet) and you can go through the code and other information to make yourself familiar with.

This will push you fast forward towards understanding how this all works together with game.
[Check our smart contract here](https://sepolia.etherscan.io/address/0x52fa3dEFa9358E9164a5fc5528C31351210E3039#code)

### Prerequisites
Unity Version: 2022.3+
<br/><br/>

### 🔧 Install Package 
```
1. In Unity Editor, Expand Window Menu -> Click on Package Manager.
2. Press + dropdown on the top left of Package Manager window and select "Add package from git URL".
3. Copy and paste https://github.com/ektishafgithub/ektishaf-api-unity.git and press Add.
```
That's it the package will be added to Unity and all core functions can then be executed through **RequestManager.cs** file.
<br/><br/>

### 🔨 Unity Documentation
This is a Unity documentation explaining each of the core functions in **RequestManager.cs** file.

#### Host
Checks if the host is alive and returns a status code (200) with a welcome message showing readiness for blockchain interaction.

```cs
  public void Host(Action<bool, string> callback)
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `callback` | `Action<bool, string>` | **Response**. A response returned with success and result values respectively |

| Response | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `string` | `string` | **Returns**. A string containing a welcome message. |

#### Register
Creates a new in-game wallet for the user based on the specified unique complex password.

```cs
  public void Register(string password, Action<bool, string, string, string> callback)
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `password` | `string` | **Required**. A new unique complex password to be specified for the wallet |
| `callback` | `Action<bool, string, string, string>` | **Response**. A response returned with success, address, ticket and error values respectively |

| Response | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `Json Object` | `string` | **Returns**. A json object string result containing a wallet address and it's ticket |


#### Login
Logins with an existing wallet based on the ticket and the password.

```cs
  public void Login(string ticket, string password, Action<bool, string, string, string> callback)
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `ticket` | `string` | **Required**. A valid ticket that was issued by the API |
| `password` | `string` | **Required**. The unique complex password of the wallet |
| `callback` | `Action<bool, string, string, string>` | **Response**. A response returned with success, address, ticket and error values respectively |

| Response | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `Json Object` | `string` | **Returns**. A json object string result containing a wallet address and it's ticket |

#### External
Uses an external wallet to login based on the specified private key.

```cs
  public void External(string privateKey, string password, Action<bool, string, string, string> callback)
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `privateKey` | `string` | **Required**. Private key of the external wallet to be used in game |
| `password` | `string` | **Required**. A new unique complex password to be specified for the external wallet |
| `callback` | `Action<bool, string, string, string>` | **Response**. A response returned with success, address, ticket and error values respectively |

| Response | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `Json Object` | `string` | **Returns**. A json object string result containing a wallet address and it's ticket |

#### Sign
Signs a message with the logged in wallet.

```cs
  public void Sign(string message, Action<bool, string, string, string> callback, string ticket)
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `message` | `string` | **Required**. Private key of the external wallet to be used in game |
| `callback` | `Action<bool, string, string, string>` | **Response**. A response returned with success, message, signature and error values respectively |
| `ticket` | `string` | **Required**. A valid ticket that was issued by the API |

| Response | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `Json Object` | `string` | **Returns**. A json object string data containing the message and it's signature |

#### Verify
Verifies the signed message.

```cs
  public void Verify(string address, string message, string signature, Action<bool, bool, string> callback)
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `address` | `string` | **Required**. The wallet address that has signed the message |
| `message` | `string` | **Required**. The message that was signed |
| `signature` | `string` | **Required**. The signature of the signed message |
| `callback` | `Action<bool, bool, string>` | **Response**. A response returned with success, verification and error values respectively |

| Response | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `Json Object` | `string` | **Returns**. A json object string data containing a verification boolean, where true means verified |

#### Balance
Gets the eth balance on the wallet.

```cs
  public void Balance(string rpc, Action<bool, BigInteger, string> callback, string ticket)
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `rpc` | `string` | **Required**. The wallet address that has signed the message |
| `callback` | `Action<bool, BigInteger, string>` | **Response**. A response returned with success, balance and error values respectively |
| `ticket` | `string` | **Required**. A valid ticket that was issued by the API |

| Response | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `Json Object` | `string` | **Returns**. A json object string data containing the balance |

#### ABI
Gets a Human-Readable ABI based on the specified Contract ABI.

```cs
  public void ABI(string abi, bool minimal, Action<bool, string[], string> callback)
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `rpc` | `string` | **Required**. The wallet address that has signed the message |
| `callback` | `Action<bool, string[], string>` | **Response**. A response returned with success, abi and error values respectively |
| `ticket` | `string` | **Required**. A valid ticket that was issued by the API |

| Response | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `Json Object` | `string` | **Returns**. A json object string data containing the Human-Readable ABI |

#### Read
Executes a read-only function in a smart contract and returns any data if applicable.

```cs
  public void Read(string rpc, string contract, string abi, string function, Action<bool, JObject, string> callback, string ticket, params object[] args)
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `rpc` | `string` | **Required**. The rpc to be used for the blockchain interaction |
| `contract` | `string` | **Required**. The address of the contract |
| `abi` | `string` | **Required**. The abi of the contract or the Human-Readable ABI of the function |
| `function` | `string` | **Required**. The name of the function in the contract |
| `callback` | `Action<bool, JObject, string>` | **Response**. A response returned with success, jsonObject and error values respectively |
| `ticket` | `string` | **Required**. A valid ticket that was issued by the API |
| `args` | `string` | **Required**. The arguments required by the function in the contract |

| Response | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `Json Object` | `string` | **Returns**. A json object string data containing the result of the function executed. |

#### Write
Executes a state-changing function in a smart contract and returns any transaction data if applicable.

```cs
  public void Write(string rpc, string contract, string abi, string function, Action<bool, JObject, string> callback, string ticket, params object[] args)
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `rpc` | `string` | **Required**. The rpc to be used for the blockchain interaction |
| `contract` | `string` | **Required**. The address of the contract |
| `abi` | `string` | **Required**. The abi of the contract or the Human-Readable ABI of the function |
| `function` | `string` | **Required**. The name of the function in the contract |
| `callback` | `Action<bool, JObject, string>` | **Response**. A response returned with success, jsonObject and error values respectively |
| `ticket` | `string` | **Required**. A valid ticket that was issued by the API |
| `args` | `string` | **Required**. The arguments required by the function in the contract |

| Response | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `Json Object` | `string` | **Returns**. A json object string data containing the result of the function executed. |

## Authors

- [@ektishafgithub](https://www.github.com/ektishafgithub)

## 🔗 Links
[![linkedin](https://img.shields.io/badge/linkedin-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/ektishaf)

## Support

For support, email us at mail@ektishaf.com

