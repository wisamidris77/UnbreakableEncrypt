# Unbreakable Encrypter

Unbreakable Encrypter is a command-line tool designed for secure encryption and decryption of text using unique, automatically generated keys. This application provides a user-friendly interface, making it easy to protect sensitive information.

![image](https://github.com/user-attachments/assets/3d0b1f3c-22b8-44a6-b183-323c5da75604)


## Features

- **User-Friendly Interface**: Simple command-line prompts for easy navigation.
- **Secure Key Generation**: Each encryption operation generates unique keys for enhanced security.
- **Cross-Platform**: Built on .NET, it can run on any platform that supports the .NET SDK.
- **Open Source**: Contributions are welcome to improve and expand the functionality.

## Getting Started

### Prerequisites

Before you begin, ensure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/download) (version 5.0 or higher)

### Installation Steps

1. **Clone the repository**:

   ```bash
   git clone https://github.com/wisamidris7/unbreakable-encrypter.git
   cd unbreakable-encrypter
   ```

2. **Build the project**:

   ```bash
   dotnet build
   ```

3. **Run the application**:

   ```bash
   dotnet run
   ```

## Usage Instructions

### Encryption

1. Launch the application.
2. Press `E` when prompted to encrypt text.
3. Input the text you wish to encrypt when prompted.
4. The application will display the encrypted text along with three generated keys.

### Decryption

1. Launch the application.
2. Press `D` when prompted to decrypt text.
3. Input the encrypted text and the three keys generated during encryption.
4. The application will output the decrypted text.

### Example Workflow

- **Encrypting**:
  - **Input**: `Hello, World!`
  - **Output**: 
    - Encrypted Word: `XyZ1234...`
    - Key1: `ab12...`
    - Key2: `cd34...`
    - Key3: `ef56...`

- **Decrypting**:
  - **Input**: Encrypted Word and Keys.
  - **Output**: `Hello, World!`

## Key Concepts

- **Keys**: Each encryption operation generates three unique keys that must be stored securely for later decryption. Without these keys, the encrypted text cannot be decrypted.

## Contributing

We welcome contributions! To contribute:

1. Fork the repository.
2. Create a new branch for your feature or fix.
3. Make your changes and test them.
4. Submit a pull request describing your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Contact

For questions, suggestions, or feedback, please reach out to [Wisam Idris](https://github.com/wisamidris77).
