import { Injectable } from '@angular/core';
import { User, UserManager, UserManagerSettings } from 'oidc-client-ts';
import { BehaviorSubject, Observable, tap} from 'rxjs';
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  authUser = new BehaviorSubject<User | null>(null);
  userManager: UserManager;

  constructor() {
    const userManagerSettings : UserManagerSettings = {
      authority : environment.stsAuthority,
      client_id : environment.clientId,
      redirect_uri : `${location.origin}/login-callback`,
      post_logout_redirect_uri: `${location.origin}/`,
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

  async handleUserLoggedOut() {
    await this.userManager!.removeUser();
    this.authUser.next(null);
  }

  async handleUserLoggedIn() {
    const user = await this.userManager.getUser();
    this.authUser.next(user);
  }
}
