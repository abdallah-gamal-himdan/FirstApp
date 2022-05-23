import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { Observable } from 'rxjs';
import { Pagination } from 'src/app/_models/pagiantion';
import { Userprams } from 'src/app/_models/userPrams';
import { AccountService } from 'src/app/_services/account.service';
import { take } from 'rxjs/operators';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members : Member[];
  pagination : Pagination;
  userparams : Userprams;
  user : User;
  genderList = [{value:'male',display:'males'},{value:'female',display:'Females'}];


  constructor(private memberService: MembersService) { 
this.userparams  = memberService.getUserprams();
  }

  ngOnInit(): void {
    this.loadMemeber();
  }
  loadMemeber()
  {
    this.memberService.setUserprams(this.userparams);
    this.memberService.getMembers(this.userparams).subscribe(res=>
    {
      this.members = res.result;
      this.pagination = res.pagination;
      debugger;

    })
  }

  resetFilters()
  {
    this.userparams = this.memberService.resetUserprams();
    this.loadMemeber();
  }

  pageChanged(event : any)
  {
    this.userparams.pageNumber  = event.page;
    this.memberService.setUserprams(this.userparams);
    debugger;

    this.loadMemeber();
  }
}
