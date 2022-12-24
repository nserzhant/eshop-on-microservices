import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap} from 'rxjs';
import { ChangePasswordDTO, EmployeeAccountClient, Roles, UserDTO } from './api/users.api.client';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  user = new BehaviorSubject<AuthenticatedUser | null>(null);

  constructor(private employeeAccountClient: EmployeeAccountClient) { }

  login(email: string, password: string) : Observable<string> {
    const userDto = new UserDTO();
    userDto.password = password;
    userDto.email = email;
    return this.employeeAccountClient.login(userDto).pipe(
      tap(token => {
        this.handleAuthentication(token);
      })
    );
  }

  register(email: string, password: string): Observable<void> {
    const userDto = new UserDTO();
    userDto.password = password;
    userDto.email = email;
    return this.employeeAccountClient.regiser(userDto).pipe(
      tap(() => this.logout())
    );
  }

  changeEmail(email: string, password: string): Observable<void> {
    const userDto = new UserDTO();
    userDto.password = password;
    userDto.email = email;
    return this.employeeAccountClient.changeEmail(userDto).pipe(
      tap(() => this.logout())
    );
  }

  changePassword(currentPassword: string, newPassword: string): Observable<void> {
    const changePasswordDTO = new ChangePasswordDTO();
    changePasswordDTO.oldPassword = currentPassword;
    changePasswordDTO.newPassword = newPassword;
    return this.employeeAccountClient.changePassword(changePasswordDTO).pipe(
      tap(() => this.logout())
    );
  }

  logout() {
    this.user.next(null);
  }

  handleAuthentication(idToken: string) {
    const authenticatedUser = new AuthenticatedUser(idToken);
    this.user.next(authenticatedUser);
  }
}

export class AuthenticatedUser {
  email?: string;
  id?: string;
  roles?: Roles[];
  _tokenExpirationDate?: Date;

  constructor(
    private _token: string
  ) {
      var base64Url = _token.split('.')[1];
      var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      var jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function(c) {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
  
      const tokenJSON = JSON.parse(jsonPayload);
      console.log(tokenJSON);
      this.id = tokenJSON['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
      this.email = tokenJSON["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
      const date = new Date(0);
      date.setUTCSeconds(tokenJSON.exp);
      this._tokenExpirationDate = date;
      const roles : string = tokenJSON["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
      this.roles = new Array<Roles>();
      if(Object.values(Roles).indexOf(roles) >= 0) {
        const role  = Roles[roles as keyof typeof Roles];
        this.roles.push(role);
      }
  }

  get token() {
    if (!this._tokenExpirationDate || new Date() > this._tokenExpirationDate) {
      return '';
    }
    return this._token;
  }
}
