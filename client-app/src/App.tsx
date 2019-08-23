import React from 'react'
import './App.css'
import axios from 'axios'
import { Header, Icon, List, Container } from 'semantic-ui-react';


class App extends React.Component {

  state = { values: [] }
  async componentDidMount() {
    const response = await axios.get('http://localhost:5000/api/values')
    this.setState({ values: response.data })
  }
  
  render() {
    return (
      <div>
        <Container>
          <Header as='h2'>
            <Icon name='users' />
            <Header.Content>Reactivities</Header.Content>
          </Header>
          <List>
            {this.state.values.map((value: any) => <List.Item key={value.id}>{value.name}</List.Item>)}
          </List>
          </Container>
      </div>
    );
  }
}

export default App
