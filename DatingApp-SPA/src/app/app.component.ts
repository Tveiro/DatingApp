import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'app';
  jwtHelper = new JwtHelperService();

  constructor(private authService: AuthService) {}

  // when this component loads, this is the one we'll use to get the token from local storage.
  // if we have the token, then we will set decoded token in authservice to that token, but decoded.
  ngOnInit() {
    const token = localStorage.getItem('token');
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (token) {
      // when the app loads, the authservice is going to have decodedtoken, as long as the token is available in the local storage.
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if (user) {
      this.authService.currentUser = user;
      // this is going to update the current photo in authservice with the currentuser photo that we're storing inside our localstorage
      this.authService.changeMemberPhoto(user.photoUrl);
    }
  }
}
