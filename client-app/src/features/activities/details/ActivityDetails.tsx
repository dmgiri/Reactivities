import React, { useContext, useEffect } from 'react'
import { RouteComponentProps } from 'react-router-dom'
import { Grid } from 'semantic-ui-react'
import { observer } from 'mobx-react-lite'
import LoaderComponent from '../../../app/layout/LoaderComponent'
import ActivityDetailsHeader from './ActivityDetailsHeader'
import ActivityDetailsInfo from './ActivityDetailsInfo'
import ActivityDetailsChat from './ActivityDetailsChat'
import ActivityDetailsSidebar from './ActivityDetailsSidebar'
import { RootStoreContext } from '../../../app/stores/rootStore';

interface DetailParams { id: string }

const ActivityDetails: React.FC<RouteComponentProps<DetailParams>> = ({ match }) => {

  const rootStore = useContext(RootStoreContext)
  const { activity, loadActivity, loadingInitial } = rootStore.activityStore

  useEffect(() => { loadActivity(match.params.id)}, [loadActivity, match.params.id])
  if (loadingInitial || !activity) return <LoaderComponent  content='Loading activity...' />

  return (
    <Grid>
      <Grid.Column width={10}>
        <ActivityDetailsHeader activity={activity} />
        <ActivityDetailsInfo activity={activity}/>
        <ActivityDetailsChat />
      </Grid.Column>
      <Grid.Column width={6}>
        <ActivityDetailsSidebar attendees={activity.attendees} />
      </Grid.Column>
   </Grid>
  )
}

export default observer(ActivityDetails)
