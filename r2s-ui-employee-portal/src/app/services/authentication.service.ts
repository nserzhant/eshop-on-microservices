import { Injectable } from '@angular/core';
import { User, UserManager, UserManagerSettings } from 'oidc-client-ts';
import { BehaviorSubject, Observable, tap} from 'rxjs';
import { environment } from 'src/environments/environment';
import { ChangePasswordDTO, EmployeeAccountClient, EmployeeDTO } from './api/employee.api.client';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  authUser = new BehaviorSubject<User | null>(null);
  userManager: UserManager;

  constructor(private employeeAccountClient: EmployeeAccountClient) {
    const userManagerSettings : UserManagerSettings = {
      authority : environment.stsAuthority,
      client_id : environment.clientId,
      redirect_uri : `${environment.clientRoot}login-callback`,
      post_logout_redirect_uri: `${environment.clientRoot}`, 
      response_type : 'code',
      scope : 'openid api roles offline_access'
    };
    
    this.userManager = new UserManager(userManagerSettings);
    this.userManager.events.addUserSignedIn(()=>this.handleUserLoggedIn());
    this.userManager.events.addUserSignedOut(()=>this.handleUserLoggedOut());
    
    this.userManager.getUser().then(user => {
      if(user?.expired) {
        this.userManager.removeUser();
      } else {
        this.authUser.next(user);
      }
    });  
  }

  public login(): Promise<void> {
    return this.userManager.signinRedirect();
  }

  public logout(): Promise<void> {
    return this.userManager.signoutRedirect();
  }

  register(email: string, password: string): Observable<void> {
    const employeeDTO = new EmployeeDTO();
    employeeDTO.password = password;
    employeeDTO.email = email;

    return this.employeeAccountClient.regiser(employeeDTO);
  }

  changeEmail(email: string, password: string): Observable<void> {
    const employeeDTO = new EmployeeDTO();
    employeeDTO.password = password;
    employeeDTO.email = email;

    return this.employeeAccountClient.changeEmail(employeeDTO).pipe(
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

  async handleUserLoggedOut() {
    await this.userManager!.removeUser();
    this.authUser.next(null);
  }

  async handleUserLoggedIn() {
    const user = await this.userManager.getUser();
    this.authUser.next(user);
  }
}
