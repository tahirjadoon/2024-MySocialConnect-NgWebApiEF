import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Subscription, take } from 'rxjs';
import { FileUploader } from 'ng2-file-upload';
import { ToastrService } from 'ngx-toastr';

import { HelperService } from '../../../core/services/helper.service';
import { AccountService } from '../../../core/services/account.service';
import { MemberService } from '../../../core/services/member.service';

import { LoggedInUserDto } from '../../../core/models-interfaces/logged-in-user-dto.model';
import { UserDto } from '../../../core/models-interfaces/user-dto.model';
import { PhotoDto } from '../../../core/models-interfaces/photo-dto.model';

import { AppConstants } from '../../../core/constants/app-constants';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit, OnDestroy {

  @Input() member: UserDto | undefined;

  title = "PhotoEditor";
  user: LoggedInUserDto | undefined;

  /* file uplaoder properties start */
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver = false;
  /* file uplaoder properties end */

  setPhotoMainSubscription!: Subscription;
  deletePhotoSubscriptoion!: Subscription;

  constructor(private helperService: HelperService, private toastr: ToastrService, 
      private accountService: AccountService, private memberService: MemberService){
    //fill the logged in user, taking 1 so no need to unsubscribe
    this.accountService.currentLoggedInUser$.pipe(take(1)).subscribe({
      next: user => {
        if(user) this.user = user;
      }
    });
  }

  ngOnDestroy(): void {
    if(this.setPhotoMainSubscription) this.setPhotoMainSubscription.unsubscribe();
    if(this.deletePhotoSubscriptoion) this.deletePhotoSubscriptoion.unsubscribe();
  }

  ngOnInit(): void {
    this.printInfo();
    this.initilizeUploader();
  }

  //to set the drop zone, name must be exact
  fileOverBase(e: any){
    this.hasBaseDropZoneOver = e;
  }

  private printInfo(){
    this.helperService.logIf(`***${this.title}***`);
    this.helperService.logIf(`url: ${this.helperService.urlUserPhotoAdd}`);
    this.helperService.logIf(`user: ${this.user?.userName}`);
  }

  /* initilizeUploader start */
  initilizeUploader(){

    const maxFileSize = 10 * 1024 * 1024; //10 mb image only

    //setup
    this.uploader = new FileUploader({
      url: this.helperService.urlUserPhotoAdd,
      authToken: `${AppConstants.Bearer}${this.user?.token}`,
      isHTML5: true, 
      allowedFileType: ['image'], //jpeg, png, gif etc are allowed
      removeAfterUpload: true, //remove the file from drop zone
      autoUpload: false, //user will need to click the button 
      maxFileSize: maxFileSize, 
    });

    //we are using bearer token. Otherwise will need to adjust the cors configuration and allow credentials to go with file
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false
    }

    //push the photo to the members.photos array after successful upload
    //it is making the users/add/photo call which is returning the added photo. Photo is in the response
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if(response){
        const photo = JSON.parse(response);
        this.member?.photos.push(photo);
        if(photo.isMain){
          if(this.user) this.user.mainPhotoUrl = photo.url;
          if(this.member) this.member.photoUrl = photo.url;
          this.accountService.setAndFireCurrentUser(this.user!);
        }
      }
    }

    //when the file is not added display error message
    this.uploader.onWhenAddingFileFailed = (item, filter) => {
      switch(filter.name){
        case 'fileSize': {
          const fileSizeError = `${item.name} is ${this.formatBytes(item.size)}. Max allowed is ${this.formatBytes(maxFileSize)}`;
          this.toastr.error(fileSizeError, `Error: ${filter.name}`);
          break;
        }
        default: {
          this.toastr.error(`Error while uploading file ${item.name}`, `Error: ${filter.name}`);
          break;
        }
      }
    }

  }
  /* initilizeUploader end */

  private formatBytes(bytes:number, decimals?: number) {
    if (bytes == 0) return '0 Bytes';
    const k = 1024,
      dm = decimals || 2,
      sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'],
      i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
  }

  onSetMainPhoto(photo: PhotoDto){
    this.setPhotoMainSubscription = this.memberService.setMainPhoto(photo.id).subscribe({
      next: () => {
        if(this.user && this.member) {
          this.user.mainPhotoUrl = photo.url;
          this.accountService.setAndFireCurrentUser(this.user);

          this.member.photoUrl = photo.url;
          this.member.photos.forEach(p => {
            if(p.isMain) p.isMain = false;
            if(p.id === photo.id) p.isMain = true;
          });
        }
      },
      error: e => {},
      complete: () => {}
    });
  }

  onDeletePhoto(photo: PhotoDto){
    this.deletePhotoSubscriptoion = this.memberService.deletePhoto(photo.id).subscribe({
      next: () => {
        //remove the photos from the member
        if(this.member && this.member.photos)
          this.member.photos = this.member.photos.filter(x => x.id !== photo.id);
      },
      error: e => {},
      complete: () => {}
    })
  }

}
