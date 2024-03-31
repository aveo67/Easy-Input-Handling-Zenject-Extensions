using System;
using Zenject;

namespace EasyInputHandling.ZenjectExtensions
{
	/// <summary>
	/// Методы расширения модуля для регистрации сервисов обработки ввода в контейнере зависимостей
	/// </summary>
	public static class InputDiExtensions
	{
		public static DiContainer BindInputHandlerFactoryFromProfile<TProfile, TContext>(this DiContainer container)
			where TProfile : InputProfile<TContext>
		{
			if (container.HasBinding<IInputFactory<TContext>>())
				throw new InvalidOperationException($"Container contains binding of Input Handler for context {typeof(TContext).Name}");

			container.Bind<TProfile>().AsTransient();

			container.Bind<IInputFactory<TContext>>().FromMethod(() =>
			{
				var profile = container.Resolve<TProfile>();

				profile.Configure();

				return profile.Build();
			}).AsTransient();

			return container;
		}

		public static DiContainer BindExtensionProfileForInputHandler<TProfile, TContext>(this DiContainer container)
			where TProfile : InputExtensionProfile<TContext>
		{
			if (container.HasBinding<TProfile>())
				throw new InvalidOperationException($"Container contains binding of profile {typeof(TProfile).Name}");

			container.Bind<TProfile>().AsTransient();

			container.Bind<Action<IInputFactoryBuilder<TContext>>>().FromMethod(() =>
			{
				var profile = container.Resolve<TProfile>();

				return profile.Configure();
			});

			return container;
		}

		/// <summary>
		/// Регистрация в контейнере зависимостей фабрики обработчика ввода для контекста <typeparamref name="TContext"/>
		/// </summary>
		/// <typeparam name="TContext">Контекст обработки ввода</typeparam>
		/// <param name="container">Контейнер зависимостей</param>
		/// <param name="expression">Выражение которое сконструирует экземпляр фабрики обработчика ввода</param>
		/// <exception cref="InvalidOperationException">Возникает если фабрика для контекста <typeparamref name="TContext"/> уже была зарегистрирована</exception>
		public static DiContainer BindInputFor<TContext>(this DiContainer container, Action<IInputFactoryBuilder<TContext>> expression)
		{
			if (container.HasBinding<IInputFactory<TContext>>())
				throw new InvalidOperationException($"Container contains binding of Input Handler for context {typeof(TContext).Name}");

			container.Bind<InputImplicitProfile<TContext>>().AsTransient();

			container.Bind<Action<IInputFactoryBuilder<TContext>>>().FromInstance(expression);

			container.Bind<IInputFactory<TContext>>().FromMethod(() =>
			{
				var profile = container.Resolve<InputImplicitProfile<TContext>>();

				profile.Configure(expression);

				return profile.Build();
			});

			return container;
		}

		/// <summary>
		/// Расширяет условия обработки ввода для контекста <typeparamref name="TContext"/>
		/// </summary>
		/// <typeparam name="TContext">Контекст обработки ввода</typeparam>
		/// <param name="container">Контейнер зависимостей</param>
		/// <param name="expression">Выражение которое расширит парвила создания обработчика ввода</param>
		public static DiContainer ExtentInputFor<TContext>(this DiContainer container, Action<IInputFactoryBuilder<TContext>> expression)
		{
			container.Bind<Action<IInputFactoryBuilder<TContext>>>().FromInstance(expression);
			return container;
		}

		/// <summary>
		/// Расширяет условия обработки ввода для контекста <typeparamref name="TContext"/>. Передает SignalBus в качестве доп.параметра для обработки событий вне контекста <typeparamref name="TContext"/>
		/// </summary>
		/// <typeparam name="TContext">Контекст обработки ввода</typeparam>
		/// <param name="container">Контейнер зависимостей</param>
		/// <param name="expression">Выражение которое расширит парвила создания обработчика ввода</param>
		public static DiContainer ExtentInputFor<TContext>(this DiContainer container, Action<IInputFactoryBuilder<TContext>, SignalBus> expression)
		{
			Action<IInputFactoryBuilder<TContext>> e = b => expression(b, container.Resolve<SignalBus>());
			container.Bind<Action<IInputFactoryBuilder<TContext>>>().FromInstance(e);
			return container;
		}

		/// <summary>
		/// Расширяет условия обработки ввода для контекста <typeparamref name="TContext"/>. Перезает в качестве доп.параметра дополнительный контекст обработки ввода типа <typeparamref name="TExtContext"/>
		/// </summary>
		/// <typeparam name="TContext">Контекст обработки ввода</typeparam>
		/// <typeparam name="TExtContext">Дополнительный контекст обработки ввода. Должен быть рарегистрирован в контейнере зависимостей</typeparam>
		/// <param name="container">Контейнер зависимостей</param>
		/// <param name="expression">Выражение которое расширит парвила создания обработчика ввода</param>
		public static DiContainer ExtentInputFor<TContext, TExtContext>(this DiContainer container, Action<IInputFactoryBuilder<TContext>, TExtContext> expression)
		{
			Action<IInputFactoryBuilder<TContext>> e = b => expression(b, container.Resolve<TExtContext>());
			container.Bind<Action<IInputFactoryBuilder<TContext>>>().FromInstance(e);
			return container;
		}
	}
}
