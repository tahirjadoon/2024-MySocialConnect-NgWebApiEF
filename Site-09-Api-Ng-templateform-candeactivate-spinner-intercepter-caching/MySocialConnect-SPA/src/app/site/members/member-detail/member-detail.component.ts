import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';

import { UserDto } from '../../../core/models-interfaces/user-dto.model';

import { MemberService } from '../../../core/services/member.service';


@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule]
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  
  member: UserDto | undefined;
  guidParam: string | null = "";
  nameParam: string | null = "";

  images: GalleryItem[] = [];

  memberSubscription!: Subscription;
  
  constructor(private memberService: MemberService, 
              private activatedRoute: ActivatedRoute, 
              private router: Router,
              private toastr: ToastrService){}

  ngOnDestroy(): void {
    if(this.memberSubscription) this.memberSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.guidParam = this.activatedRoute.snapshot.paramMap.get('guid');
    this.nameParam = this.activatedRoute.snapshot.paramMap.get("name");

    if(!this.guidParam)
      this.naviagteToMember("GUID not received on the page", "User Error");

    this.loadMember();
  }

  loadMember(){
    this.memberSubscription = this.memberService.getMemberByGuid(this.guidParam!).subscribe({
      next: (member: UserDto) => {
        if(this.nameParam?.toLowerCase() !== member.displayName.toLowerCase())
          this.naviagteToMember("Mismatched user", "User Error");

        this.member = member;
        this.getImages();
      },
      error: e => {},
      complete: () => {}
    })
  }

  getImages(){
    if(!this.member || !this.member.photos || this.member.photos.length <= 0) return;

    for(const photo of this.member.photos){
      this.images.push(new ImageItem({src: photo.url, thumb: photo.url} ))
    }
  }

  naviagteToMember(error: string, errorTitle: string){
    if(error)
      this.toastr.error(error, errorTitle);
    this.router.navigate(["/members"]);
  }

}
