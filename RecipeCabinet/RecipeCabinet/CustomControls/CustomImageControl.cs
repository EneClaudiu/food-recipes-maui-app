namespace RecipeCabinet.CustomControls
{
    public class CustomImageControl : ContentView
    {
        public static readonly BindableProperty ImageSrcProperty = BindableProperty.Create(
            nameof(ImageSrc), typeof(string), typeof(CustomImageControl), default(string), propertyChanged: OnImageSrcChanged);

        public static readonly BindableProperty AspectProperty = BindableProperty.Create(
            nameof(Aspect), typeof(Aspect), typeof(CustomImageControl), Aspect.AspectFit, propertyChanged: OnAspectChanged);

        private readonly Image _imageControl;

        public string ImageSrc
        {
            get => (string)GetValue(ImageSrcProperty);
            set => SetValue(ImageSrcProperty, value);
        }

        public Aspect Aspect
        {
            get => (Aspect)GetValue(AspectProperty);
            set => SetValue(AspectProperty, value);
        }

        public CustomImageControl()
        {
            _imageControl = new Image();
            Content = _imageControl;
        }

        private static void OnImageSrcChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomImageControl)bindable;
            var source = (string)newValue;

            if (!string.IsNullOrEmpty(source))
            {
                if (Uri.TryCreate(source, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                {
                    control._imageControl.Source = ImageSource.FromUri(uriResult);
                }
                else
                {
                    control._imageControl.Source = ImageSource.FromFile(source);
                }
            }
        }

        private static void OnAspectChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomImageControl)bindable;
            var aspect = (Aspect)newValue;
            control._imageControl.Aspect = aspect;
        }
    }
}
