# Easy Input Handling. Zenject Extension Methods

Contains several extension methods for binding `Input Handler` factories to Zenject DI-container.

## Requirements

- Unity 2023.1 or newer;
- New Input System package;
- Zenject;
- Easy Input Handling package.

## Getting started

There are two options to bind factory to container: with profile or without.

### Binding with Profile

Create an input handling profile the same way as in this [documetation](https://github.com/aveo67/Easy-Input-Handling?tab=readme-ov-file#step-2-create-input-handler-profile). Everything you need next is to call `BindInputHandlerFactoryFromProfile` extension method for `DiContainer` class instance. Extension input handling profile is binding by calling `BindExtensionProfileForInputHandler` extension method. For example you can do this in an `Installer`.

``` CSharp
internal class ExampleInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		Container.
			// Main profile binding
			BindInputHandlerFactoryFromProfile<ExampleInputProfile, ExampleContext>().
			// Additional profile binding
			BindExtensionProfileForInputHandler<ExampleInputExtensionProfile, ExampleContext>();
	}
}
```

> Attempt to bind two or more profiles for one context type will throw an exception. You should configure all behavior in one profile or use one of ways to extend input handling if it is impossible.

### Direct binding to container

If you use this package, creating a profile is not necessary. It is enough to use an extension method to configure an input directly into the di-container. Call `BindInputFor` extension method on a di-container instance and configure input right there. Moreover, you can extend input handling by `ExtentInputFor` extension method in the same way as an extension profile.

``` CSharp
internal class ExampleInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		Container.
			BindInputFor<ExampleContext>(builder => builder.
				UseHandling<MouseActions, ExampleContext>(builder => builder.
					HandleInputActionPerformed(x => x.MouseActionsMap.LeftButtomClick, context => context.Print()).
			ExtentInputFor<ExampleContext>(builder => builder.
				UseHandling<MouseActions, ExampleContext>(builder => builder .
					HandleInputActionPerformed(x => x.MouseActionsMap.LeftButtomClick, context => Debug.Log("Additional behaviour"))));
	}
}
```
> Attempt to use `BindInputFor` method two or more times for one context type will throw an exception. You should configure all behavior in one expression or use one of ways to extend input handling if it is impossible.

### When might the input handling extension be useful?

For example, you have an input handling in one assembly and you can't change it or haven't got access but you need to add some additional behavior. You can do it by one of those ways in other assembly.

> Note that if container haven't got main input handling binding the extension profile or `ExtentInputFor` extension method wouldn't work. So first you need to bind input handling profile by `BindInputHandlerFactoryFromProfile` method calling or using `BindInputFor` method and then bind the extension.

> Attempt to use `BindInputHandlerFactoryFromProfile` and `BindInputFor` methods for one context type at the same time will throw an exception. You should choose something one and then use one of the way to extend input handling.

### Dependency injection and using

When the handling configured and bound you can inject an input handler factory in your class. Create an instance of input handler by calling `Create` method. It takes an instance of context as an argument.

``` CSharp
internal class Game : IDisposable
{
	private readonly IInput _inputHandler;

	public Game(IInputFactory<Character> factory)
	{
		// Creating an instance of input handler
		_inputHandler = factory.Create(new Character());

		// Enable handling
		_inputHandler.Enable();
	}

	public void Dispose()
	{
		// Disposing at the end
		_inputHandler.Dispose();
	}
}
```
The same for `MonoBehaviour` classes:

``` CSharp
internal class Game : MonoBehaviour
{
	private IInput _inputHandler;

	[Inject]
	private void Construct(IInputFactory<Character> factory)
	{
		_inputHandler = factory.Create(new Character());
	}

	private void Start()
	{
		_inputHandler.Enable();
	}

	private void OnDestroy()
	{
		_inputHandler.Dispose();
	}
}
```
## Installation

Unfortunately Unity doesn't support git dependency for git packages so you should install both packages manually.

Step 1. Install the [Easy Input Handling](https://github.com/aveo67/Easy-Input-Handling?tab=readme-ov-file#installation) package.

Step 2. Open the `Package Manager` window and click `+`. Choose `Install package from git URL...` and paste "https://github.com/aveo67/Easy-Input-Handling-Zenject-Extensions.git". Click `Install` button.

> Perhaps this may require installing the zenject package manually. You can do it by [OpenUPM](https://openupm.com/packages/com.svermeulen.extenject/) for example.

## Examples

Soon

## License

This library is under the MIT License.
