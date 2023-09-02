import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-cards',
  templateUrl: './member-cards.component.html',
  styleUrls: ['./member-cards.component.css']
})
export class MemberCardsComponent implements OnInit {
  @Input() member: Member;
  constructor(private memberService: MembersService, private toastr: ToastrService, 
    public presenceService: PresenceService) { }

  ngOnInit(): void {
  }

  addLike(member: Member)
  {
    this.memberService.addLike(member.userName).subscribe(() => {
      this.toastr.success('You have liked ' + member.knownAs);
    });
  }
}
