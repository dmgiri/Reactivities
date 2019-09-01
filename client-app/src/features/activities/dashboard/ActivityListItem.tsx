import React from 'react'
import { Link } from 'react-router-dom'
import { Item, Button, Segment, Icon, Label } from 'semantic-ui-react';
import { IActivity } from '../../../app/models/activity'
import { format } from 'date-fns'
import ActivityListItemAttendees from './ActivityListItemAttendees';


interface IProps { activity: IActivity }

const ActivityListItem: React.FC<IProps> = ({ activity }) => {

  const host = activity.attendees.filter(x => x.isHost)[0]

  return (
    <Segment.Group>
      <Segment>     
        <Item.Group>
          <Item>
            <Item.Image size='tiny' circular src={host.image || '/assets/user.png'} />
            <Item.Content>
              <Item.Header as={Link} to={`/activities/${activity.id}`}>{activity.title}</Item.Header>
              <Item.Description>Hosted by {host.displayName}</Item.Description>
              {activity.isHost && <Item.Description><Label basic color='orange' content='You are hosting this event' /></Item.Description>}
              {activity.isGoing && !activity.isHost &&
                <Item.Description><Label basic color='green' content='You are attending this event' /></Item.Description>}
            </Item.Content>
          </Item>
        </Item.Group>
      </Segment>
      <Segment>
        <Icon name='clock' /> {format(activity.date, 'h:mm a')}
        <Icon name='marker' /> {activity.venue}, {activity.city}
      </Segment>
      <Segment secondary>
        <ActivityListItemAttendees attendees={activity.attendees} />
      </Segment>
      <Segment clearing>
        <span>{activity.description}</span>
        <Button floated='right' content='View' color='blue' as={Link} to={`/activities/${activity.id}`} />
      </Segment>
    </Segment.Group>
  )
}

export default ActivityListItem
