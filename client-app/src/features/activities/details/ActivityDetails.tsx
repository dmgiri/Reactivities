import React, { useContext, useEffect } from 'react'
import { Link, RouteComponentProps } from 'react-router-dom';
import { Card, Image, Button } from 'semantic-ui-react'
import { observer } from 'mobx-react-lite'
import ActivityStore from '../../../app/stores/activityStore'
import LoaderComponent from '../../../app/layout/LoaderComponent';

interface DetailParams { id: string }

const ActivityDetails: React.FC<RouteComponentProps<DetailParams>> = ({ match, history }) => {
  
  const activityStore = useContext(ActivityStore)
  const { activity, loadActivity, loadingInitial } = activityStore

  useEffect(() => { loadActivity(match.params.id)}, [loadActivity, match.params.id])
  if (loadingInitial || !activity) return <LoaderComponent  content='Loading activity...' />

  return (
    <Card fluid>
      <Image src={`/assets/categoryImages/${activity!.category}.jpg`} wrapped ui={false} />
      <Card.Content>
        <Card.Header>{activity!.title}</Card.Header>
        <Card.Meta>
          <span>{activity!.date}</span>
        </Card.Meta>
        <Card.Description>
          {activity!.description}
        </Card.Description>
      </Card.Content>
      <Card.Content extra>
        <Button.Group widths={2}>
          <Button basic color='blue' content='Edit' as={Link} to={`/manage/${activity.id}`}></Button>
          <Button basic color='grey' content='Cancel' onClick={() => history.push('/activities')}></Button>
        </Button.Group>
      </Card.Content>
    </Card>
  )
}

export default observer(ActivityDetails)
