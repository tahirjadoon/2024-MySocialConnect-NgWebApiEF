import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { ToastrService } from 'ngx-toastr';
import { Subscription, take } from 'rxjs';
import { TimeagoModule } from 'ngx-timeago';

import { UserDto } from '../../../core/models-interfaces/user-dto.model';
import { MessageDto } from 'src/app/core/models-interfaces/message-dto.model';
import { LoggedInUserDto } from '../../../core/models-interfaces/logged-in-user-dto.model';

import { MemberService } from '../../../core/services/member.service';
import { MessageService } from '../../../core/services/message.service';
import { AccountService } from '../../../core/services/account.service';

import { PresenceHubService } from '../../../core/services/signalr/presence-hub.service';
import { MessageHubService } from '../../../core/services/signalr/message-hub.service';

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

  user: LoggedInUserDto = <LoggedInUserDto>{};

  constructor(private memberService: MemberService, 
              private activatedRoute: ActivatedRoute, 
              private router: Router,
              private toastr: ToastrService,
              private messageService: MessageService,
              public presenceHubService: PresenceHubService, 
              private messageHubService: MessageHubService, 
              private accountService: AccountService){
    //get the logged in user
    this.accountService.currentLoggedInUser$.pipe(take(1)).subscribe({
      next: (user: LoggedInUserDto | null) => {
        if(user)
          this.user = user;
      }
    });
  }

  ngOnDestroy(): void {
    if(this.memberSubscription) this.memberSubscription.unsubscribe();
    if(this.messageThreadSubscription) this.messageThreadSubscription.unsubscribe();
    if(this.memberDataFromRouteSubscription) this.memberDataFromRouteSubscription.unsubscribe();
    if(this.queryParamSubscripton) this.queryParamSubscripton.unsubscribe();

    //also stop the message hub connection. This happening below on activated tab as well 
    this.messageHubService.stopHubConnection();
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
    //user check and else put in place after message hub implementation
    if(this.activeTab.heading === "Messages" && this.user){
      //using presence hub to get the messages now rather than from the message service
      //note that we are not passing the messages from detail to messages any more
      //only create the hub connection here
      //this.loadMessages();
      this.messageHubService.createHubConnection(this.user, this.member.userName, this.member.id);  
    }
    else{
      //also happening in ngOnDestroy
      this.messageHubService.stopHubConnection();
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
