import { AbstractControl, FormGroup, ValidationErrors, ValidatorFn } from "@angular/forms";

export  function passwordMatchConfirmPasswordValidator(): ValidatorFn {
    return (control: AbstractControl) : ValidationErrors => {
      const formGroup = control as FormGroup;
      const password = formGroup?.controls['password'];
      const confirmPassword = formGroup?.controls['confirmPassword'];

      if ( confirmPassword.errors && !confirmPassword.hasError('mismatch')) {
        return {};
      }
      if (password.value && confirmPassword.value &&
          password.value !== confirmPassword.value) {
        confirmPassword.setErrors({mismatch : true});
      } else if (confirmPassword) {
        confirmPassword.setErrors(null);
      }

      return {};
    };
  }