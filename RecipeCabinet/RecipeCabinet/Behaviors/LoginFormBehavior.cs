using RecipeCabinet.Models;
using Syncfusion.Maui.DataForm;

namespace RecipeCabinet.Behaviors
{
    // This behavior is used to set the keyboard type for the email field in the login form.

    public class LoginFormBehavior: Behavior<ContentPage>
    {
        private SfDataForm _dataForm;

        protected override void OnAttachedTo(ContentPage bindable)
        {
            base.OnAttachedTo(bindable);
            this._dataForm = bindable.FindByName<SfDataForm>("loginForm");

            if (_dataForm != null)
            {
                this._dataForm.GenerateDataFormItem += OnGenerateDataFormItem;
            }
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            if (_dataForm != null)
            {
                this._dataForm.GenerateDataFormItem -= OnGenerateDataFormItem;
            }
            base.OnDetachingFrom(bindable);
        }

        private void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            if (e.DataFormItem != null && e.DataFormItem.FieldName == nameof(LoginFormModel.Email) && e.DataFormItem is DataFormTextEditorItem textItem)
            {
                textItem.Keyboard = Keyboard.Email;
            }
        }
    }
}
