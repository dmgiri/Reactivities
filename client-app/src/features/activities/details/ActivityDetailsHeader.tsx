import React, { useContext } from "react"
import { Segment, Image, Item, Button, Header } from 'semantic-ui-react'
import { IActivity } from '../../../app/models/activity'
import { observer } from 'mobx-react-lite'
import { Link } from 'react-router-dom'
import { format } from 'date-fns'
import { RootStoreContext } from '../../../app/stores/rootStore'

const activityImageStyle = { filter: 'brightness(70%)' }
const activityImageTextStyle = { position: 'absolute', bottom: '5%', left: '5%', width: '100%', height: 'auto', color: 'white' }

interface IProps { activity: IActivity }


const ActivityDetailsHeader: React.FC<IProps> = ({ activity }) => {

  const host = activity.attendees.filter(x => x.isHost)[0]

  const rootStore = useContext(RootStoreContext)
  const { attendActivity, cancelAttendance, loading } = rootStore.activityStore

  return (
    <Segment.Group>
      <Segment basic attached="top" style={{ padding: "0" }}>
        <Image src={`/assets/categoryImages/${activity.category}.jpg`} fluid style={activityImageStyle} />
        <Segment basic style={activityImageTextStyle}>
          <Item.Group>
            <Item>
              <Item.Content>
                <Header size="huge" content={activity.title} style={{ color: "white" }} />
                <p>{format(activity.date, 'eeee do MMMM')}</p>
                <p>Hosted by <strong><Link to={`/profile/${host.username}`}>{host.displayName}</Link></strong></p>
              </Item.Content>
            </Item>
          </Item.Group>
        </Segment>
      </Segment>
      <Segment clearing attached="bottom">
        {!activity.isGoing && !activity.isHost && <Button loading={loading} onClick={attendActivity} color="teal">Join Activity</Button>}
        {activity.isGoing && !activity.isHost && <Button loading={loading} onClick={cancelAttendance} >Cancel attendance</Button>}
        {activity.isHost && <Button as={Link} to={`/manage/${activity.id}`} color="orange" floated="right">Manage Event</Button>}
      </Segment>
    </Segment.Group>
  )
}

export default observer(ActivityDetailsHeader)

// notes:
// when you are hosting an event, you cannot join or cancel that event.