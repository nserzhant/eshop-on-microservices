import { Injectable, signal, computed } from '@angular/core';
import { User, UserManager, UserManagerSettings, WebStorageStateStore } from 'oidc-client-ts';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private ADMINISTRATOR_ROLE_NAME = 'Administrator';
  private SALES_MANAGER_ROLE_NAME = 'SalesManager';

  private userManager: UserManager;
  private _user = signal<User | null>(null);

  readonly isAuthenticated = computed(() => this._user() !== null);
  readonly isAdmin = computed(() => this.userRoles().includes(this.ADMINISTRATOR_ROLE_NAME));
  readonly isSalesManager = computed(() => this.userRoles().includes(this.SALES_MANAGER_ROLE_NAME));

  readonly userEmail = computed(() => {
    const user = this._user();
    return user?.profile?.email ?? '';
  });

  readonly userRoles = computed(() => {
    const user = this._user();
    if (!user) return [];

    const userRoles = user.profile['role'] ?? user.profile['roles'];
    if (!userRoles) return [];

    if (typeof userRoles === 'string') {
      return [userRoles];
    } else if (Array.isArray(userRoles)) {
      return userRoles as string[];
    }
    return [];
  });

  get accessToken() {
    return this._user()?.access_token;
  }

  signinCallback() : Promise<User | undefined> {
    return this.userManager.signinCallback();
  }

  constructor() {
    const userManagerSettings : UserManagerSettings = {
      authority : environment.stsAuthority,
      client_id : environment.clientId,
      redirect_uri : `${location.origin}/login-callback`,
      post_logout_redirect_uri: `${location.origin}/`,
      response_type : 'code',
      scope : environment.scope,
      userStore: new WebStorageStateStore({store: window.localStorage})
    };

    this.userManager = new UserManager(userManagerSettings);
    this.userManager.events.addUserSignedIn(()=>this.handleUserLoggedIn());
    this.userManager.events.addUserSignedOut(()=>this.handleUserLoggedOut());
    //Handles refresh token events
    this.userManager.events.addUserLoaded((user) => this._user.set(user));

    this.userManager.getUser().then(user => {
      if(user?.expired) {
        this.userManager.removeUser();
      } else {
        this._user.set(user);
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
    this._user.set(null);
  }

  async handleUserLoggedIn() {
    const user = await this.userManager.getUser();
    this._user.set(user);
  }
}
