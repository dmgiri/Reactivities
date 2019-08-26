import React, { useContext, useEffect } from 'react'
import { Grid } from 'semantic-ui-react'
import { observer } from 'mobx-react-lite'
import ActivityList from './ActivityList'
import ActivityStore from '../../../app/stores/activityStore'
import LoaderComponent from '../../../app/layout/LoaderComponent'


const ActivityDashboard: React.FC = () => {

  const activityStore = useContext(ActivityStore)
  useEffect(() => { activityStore.loadActivities() }, [activityStore])
  if (activityStore.loadingInitial) return <LoaderComponent content='Loading activities...' />
  
  return (
    <Grid>
      <Grid.Column width={10}>
        <ActivityList />
      </Grid.Column>
      <Grid.Column width={6}>
       <h2>Activity filters</h2>
      </Grid.Column>
    </Grid>
  )
}


export default observer(ActivityDashboard)