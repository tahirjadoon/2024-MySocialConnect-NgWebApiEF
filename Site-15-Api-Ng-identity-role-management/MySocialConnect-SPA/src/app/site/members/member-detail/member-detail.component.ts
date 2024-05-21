import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { TimeagoModule } from 'ngx-timeago';

import { UserDto } from '../../../core/models-interfaces/user-dto.model';
import { MessageDto } from 'src/app/core/models-interfaces/message-dto.model';

import { MemberService } from '../../../core/services/member.service';
import { MessageService } from 'src/app/core/services/message.service';

import { MemberMessagesComponent } from '../member-messages/member-messages.component';


@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  //adding static since now we are using route resolver to get memebr data
  //also removed the check for members in the html
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  activeTab?: TabDirective;
  messages: MessageDto[] = [];

  //after resolver so that we don't check ? in component. Initializing with empty object
  //can also do member:USerDto = {} as UserDto
  member: UserDto = <UserDto>{}; 
  guidParam: string | null = "";
  nameParam: string | null = "";

  images: GalleryItem[] = [];

  memberSubscription!: Subscription;
  messageThreadSubscription!: Subscription;

  //after resolver
  memberDataFromRouteSubscription!: Subscription;
  queryParamSubscripton!: Subscription;

  constructor(private memberService: MemberService, 
              private activatedRoute: ActivatedRoute, 
              private router: Router,
              private toastr: ToastrService,
              private messageService: MessageService){}

  ngOnDestroy(): void {
    if(this.memberSubscription) this.memberSubscription.unsubscribe();
    if(this.messageThreadSubscription) this.messageThreadSubscription.unsubscribe();
    if(this.memberDataFromRouteSubscription) this.memberDataFromRouteSubscription.unsubscribe();
    if(this.queryParamSubscripton) this.queryParamSubscripton.unsubscribe();
  }

  ngOnInit(): void {
    //old way is getting the params from the route for guid and name and then call the loadMembers
    //this.oldWay();

    //new way is using the route resolver
    this.newRouteResolver();
  }

  private oldWay(){
    this.guidParam = this.activatedRoute.snapshot.paramMap.get('guid');
    this.nameParam = this.activatedRoute.snapshot.paramMap.get("name");

    if(!this.guidParam)
      this.naviagteToMember("GUID not received on the page", "User Error");

    this.loadMember();
  }

  private newRouteResolver(){
    //we need to have the member before our component is constructed
    //for this will use the route resolver
    //check core/resolvers/member-detail and also the member-detail route in app-routing.module

    //pick member data
    this.memberDataFromRouteSubscription = this.activatedRoute.data.subscribe({
      next: data => {
        this.member = data['member'];
        if(!this.member)
          this.naviagteToMember("No member found", "Member Error");
      }
    });

    //switch tab if tab is available
    this.queryParamSubscripton = this.activatedRoute.queryParams.subscribe({
      next: params => {
        //const tab:number = +params['tab'];
        const tab: string = params['tab'];
        if(tab){
          switch (tab.toLowerCase()){
            case 'messages':
              this.onSelectTab('Messages');
              break;
          }
        }
      },
      error: e => {},
      complete: () => {}
    });

    //images
    this.getImages();
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

  onSelectTab(tabHeading: string){
    if(!this.memberTabs) return;
    this.memberTabs.tabs.find(x => x.heading === tabHeading)!.active = true;
  }

  onTabActivated(data: TabDirective){
    this.activeTab = data;
    if(this.activeTab.heading === "Messages"){
      this.loadMessages();
    }
  }

  loadMessages(){
    if(!this.member){ 
      this.toastr.error("No user", "Error");
      return;
    }

    this.messageThreadSubscription = this.messageService.getMessageThread(this.member.id).subscribe({
      next: (messages: MessageDto[]) => {
        this.messages = messages;
      },
      error: e => {},
      complete: () => {}
    });

  }

}