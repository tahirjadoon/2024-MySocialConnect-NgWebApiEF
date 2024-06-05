import { Component, OnDestroy, OnInit } from '@angular/core';

import { AdminService } from '../../../core/services/admin.service';

import { PhotoForApprovalDto } from '../../../core/models-interfaces/photo-for-approval-dto.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit, OnDestroy {

  photos: PhotoForApprovalDto[] = [];

  getPhotosSubscription!: Subscription;

  constructor(private adminService: AdminService){}

  ngOnDestroy(): void {
    if(this.getPhotosSubscription) this.getPhotosSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.getPhotosForApproval();
  }

  getPhotosForApproval(){
    this.getPhotosSubscription = this.adminService.getPhotosForApproval().subscribe({
      next: (photos: PhotoForApprovalDto[]) => {
        this.photos = photos;
      },
      error: e => {},
      complete: () => {}
    });
  }

  approvePhoto(photoId: number){
    this.adminService.approvePhoto(photoId).subscribe({
      next: () => {
        this.photos.splice(this.photos.findIndex(p => p.id == photoId), 1);
      }
    });
  }

  rejectPhoto(photoId: number){
    this.adminService.rejectPhoto(photoId).subscribe({
      next: () => {
        this.photos.splice(this.photos.findIndex(p => p.id == photoId), 1);
      }
    });
  }
}
