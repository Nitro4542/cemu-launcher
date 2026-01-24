# cemu-launcher

Automatically installs and updates nightly builds of the Cemu emulator.

## Installation

Install the [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).

Download the latest version from the [releases page](https://codeberg.org/Niitroo/cemu-launcher/releases) and install it.

Alternatively, you can download it without an installer from the same page if you'd like.

<details>
<summary>Feel free to also compile it by yourself!</summary>

### Compile it yourself

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).

2. Clone this repository and navigate into it:

    ```bash
    git clone https://codeberg.org/Niitroo/cemu-launcher.git
    cd cemu-launcher
    ```

3. Run the command according to your architecture to start compiling:

    ```bash
    dotnet publish -c Release -r win-x64
    ```

    ```bash
    dotnet publish -c Release -r win-arm64
    ```

</details>

## Usage

Instead of running Cemu, run cemu-launcher. It will automatically check for updates and install them.

It will create a portable installation of Cemu, so make sure to copy your data to it.

## License

This project is licensed under the [MIT license](LICENSE).

This project is not affiliated with Cemu or any other project.
